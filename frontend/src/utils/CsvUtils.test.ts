import { describe, it, expect, beforeEach } from 'vitest';
import { validateCsv, validateDataConstraints, calculateFileChecksum } from '../utils/CsvUtils';
import type { CsvRow } from '../types/CsvRow';
import type { ParseResult } from 'papaparse';


describe('validateCsv', () => {
    const validHeaders = [
        'Grantor First Name',
        'Grantor Middle Names',
        'Grantor Last Name',
        'VIN',
        'Registration Start Date',
        'Registration Duration',
        'SPG ACN',
        'SPG Organization Name',
    ];

    it('should return error for missing headers', () => {
        const result: ParseResult<CsvRow> = {
            meta: {
                fields: ['Grantor First Name'],
                delimiter: '',
                linebreak: '',
                aborted: false,
                truncated: false,
                cursor: 0
            },
            data: [],
            errors: [],
        };

        const validation = validateCsv(result);
        expect(validation.error).toMatch(/Missing required column/);
    });

    it('should return error for empty data', () => {
        const result: ParseResult<CsvRow> = {
            meta: {
                fields: validHeaders,
                delimiter: '',
                linebreak: '',
                aborted: false,
                truncated: false,
                cursor: 0
            },
            data: [],
            errors: [],
        };

        const validation = validateCsv(result);
        expect(validation.error).toBe('CSV file must contain at least one data row.');
    });

    it('should return error for parse errors', () => {
        const result: ParseResult<CsvRow> = {
            meta: {
                fields: validHeaders,
                delimiter: '',
                linebreak: '',
                aborted: false,
                truncated: false,
                cursor: 0
            },
            data: [{ 'Grantor First Name': 'Test' }] as CsvRow[],
            errors: [{ code: 'UndetectableDelimiter', type: 'Delimiter', message: 'test error', row: 1 }],
        };

        const validation = validateCsv(result);
        expect(validation.error).toBe('Error parsing CSV file.');
    });

    it('should return no error for valid data', () => {
        const result: ParseResult<CsvRow> = {
            meta: {
                fields: validHeaders,
                delimiter: '',
                linebreak: '',
                aborted: false,
                truncated: false,
                cursor: 0
            },
            data: [{ 'Grantor First Name': 'Test' }] as CsvRow[],
            errors: [],
        };

        const validation = validateCsv(result);
        expect(validation.error).toBeUndefined();
    });
});

describe('validateDataConstraints', () => {
    it('should mark invalid rows based on constraints', () => {
        const rows: CsvRow[] = [
            {
                "Grantor First Name": "",
                "Grantor Middle Names": "A".repeat(76),
                "Grantor Last Name": "Last",
                "VIN": "1234567890123456", // 16 characters
                "Registration Start Date": "01-01-2023",
                "Registration Duration": "10",
                "SPG ACN": "123",
                "SPG Organization Name": "Org",
                isValid: false,
            },
        ];

        const validated = validateDataConstraints(rows);
        expect(validated[0].isValid).toBe(false);
    });

    it('should mark valid rows correctly', () => {
        const rows: CsvRow[] = [
            {
                "Grantor First Name": "John",
                "Grantor Middle Names": "X",
                "Grantor Last Name": "Doe",
                "VIN": "12345678901234567",
                "Registration Start Date": "2023-01-01",
                "Registration Duration": "7",
                "SPG ACN": "123456789",
                "SPG Organization Name": "My Org",
                isValid: true,
            },
        ];

        const validated = validateDataConstraints(rows);
        expect(validated[0].isValid).toBe(true);
    });
});

describe('calculateFileChecksum', () => {
    beforeEach(() => {
        global.FileReader = class {
            public onload: (() => void) | null = null;
            public onerror: (() => void) | null = null;
            public result: ArrayBuffer | null = null;

            readAsArrayBuffer() {
                // eslint-disable-next-line @typescript-eslint/no-explicit-any
                const reader = this as any;
                const encoder = new TextEncoder();
                const buffer = encoder.encode('test content');
                reader.result = buffer.buffer;
                if (reader.onload) reader.onload();
            }
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        } as any;
    });
    it('should return a SHA-256 checksum string', async () => {
        const content = new Blob(['test content'], { type: 'text/plain' });
        const file = new File([content], 'test.csv', { type: 'text/csv' });

        const checksum = await calculateFileChecksum(file);
        expect(checksum).toMatch(/^[a-f0-9]{64}$/);
    });
});

import type { ParseResult } from 'papaparse';
import type { CsvRow } from '../types/CsvRow';
import type { CsvRowDto } from '../types/CsvRowDto';

const expectedHeaders = [
    'Grantor First Name',
    'Grantor Middle Names',
    'Grantor Last Name',
    'VIN',
    'Registration Start Date',
    'Registration Duration',
    'SPG ACN',
    'SPG Organization Name'
];

export const validateCsv = (results: ParseResult<CsvRow>): { error?: string } => {
    const headers = results.meta.fields ?? [];
    const missingHeaders = expectedHeaders.filter(header => !headers.includes(header));
    if (missingHeaders.length > 0) {
        return { error: `Missing required column(s): ${missingHeaders.join(', ')}` };
    }

    if (results.data.length === 0) {
        return { error: 'CSV file must contain at least one data row.' };
    }

    if (results.errors.length > 0) {
        return { error: 'Error parsing CSV file.' };
    }

    return {};
};

export const validateDataConstraints = (rows: CsvRow[]): CsvRowDto[] => {
    const validatedRows: CsvRowDto[] = [];
    const errorMessages: string[] = [];
    for (let i = 0; i < rows.length; i++) {
        const csvRowDto: CsvRowDto = {
            FirstName: rows[i]['Grantor First Name'],
            MiddleNames: rows[i]['Grantor Middle Names'],
            LastName: rows[i]['Grantor Last Name'],
            Vin: rows[i].VIN,
            StartDate: rows[i]['Registration Start Date'],
            Duration: rows[i]['Registration Duration'],
            Acn: rows[i]['SPG ACN'],
            OrganizationName: rows[i]['SPG Organization Name'],
            Errors: [],
            IsValid: true
        };
        csvRowDto.IsValid = true
        if (!csvRowDto.FirstName || csvRowDto.FirstName.length > 35) {
            errorMessages.push("Invalid FirstName");
            csvRowDto.IsValid = false;
        }
        if (csvRowDto.MiddleNames && csvRowDto.MiddleNames.length > 75) {
            errorMessages.push("Invalid MiddleNames");
            csvRowDto.IsValid = false;
        }
        if (!csvRowDto.LastName || csvRowDto.LastName.length > 35) {
            errorMessages.push("Invalid LastName");
            csvRowDto.IsValid = false;
        }
        if (!csvRowDto.Vin || csvRowDto.Vin.length !== 17) {
            errorMessages.push("Invalid Vin");
            csvRowDto.IsValid = false;
        }
        if (!csvRowDto.StartDate || !/^\d{4}-\d{2}-\d{2}$/.test(csvRowDto.StartDate)) {
            errorMessages.push("Invalid Registration Start Date");
            csvRowDto.IsValid = false;
        }
        if (!['7', '25', 'N/A'].includes(csvRowDto.Duration)) {
            errorMessages.push("Invalid Registration Duration");
            csvRowDto.IsValid = false;
        }
        if (!csvRowDto.Acn || !/^\d{9}$/.test(csvRowDto.Acn)) {
            errorMessages.push("Invalid Spg Acn");
            csvRowDto.IsValid = false;
        }
        if (!csvRowDto.OrganizationName || csvRowDto.OrganizationName.length > 75) {
            errorMessages.push("Invalid Spg Organization Name");
            csvRowDto.IsValid = false;
        }
        csvRowDto.Errors = errorMessages;
        validatedRows.push(csvRowDto);

    }
    return validatedRows;
};

export const calculateFileChecksum = (file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
            const buffer = reader.result as ArrayBuffer;
            crypto.subtle.digest('SHA-256', buffer).then((checksum) => {
                const checksumArray = Array.from(new Uint8Array(checksum));
                const checksumHex = checksumArray.map((b) => b.toString(16).padStart(2, '0')).join('');
                resolve(checksumHex);
            });
        };
        reader.onerror = reject;
        reader.readAsArrayBuffer(file);
    });
};
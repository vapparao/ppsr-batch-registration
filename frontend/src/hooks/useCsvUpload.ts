import { useCallback, useState } from 'react';
import Papa from 'papaparse';
import { calculateFileChecksum, validateCsv, validateDataConstraints } from '../utils/CsvUtils';
import type { CsvRow } from '../types/CsvRow';
import type { BatchResult } from '../types/BatchResult';
import type { CsvData } from '../types/CsvData';
import type { CsvRowDto } from '../types/CsvRowDto';
import { uploadCsvBatch } from '../services/BatchesService';
import { uploadRegistrations } from '../services/RegistrationsService';

const MAX_FILE_SIZE = 25 * 1024 * 1024;

interface Messages {
    error: string | null;
    warning: string | null;
    success: string | null;
}

export function useCsvUpload() {
    const [rows, setRows] = useState<CsvRowDto[]>([]);
    const [messages, setMessages] = useState<Messages>({
        error: null,
        warning: null,
        success: null,
    });
    const [uploading, setUploading] = useState(false);
    const [progress, setProgress] = useState(0);
    const [batchResults, setBatchResults] = useState<BatchResult[]>([]);
    const [inProgressResults, setInProgressResults] = useState<BatchResult[]>([]);

    const clearMessages = () =>
        setMessages({ error: null, warning: null, success: null });

    const handleFileUpload = useCallback(async (file: File) => {
        clearMessages();
        
        if (file.size > MAX_FILE_SIZE) {
            setMessages({ error: 'File is too large. Maximum allowed size is 25MB.', warning: null, success: null });
            setRows([]);
            return;
        }

        const Checksum = await calculateFileChecksum(file);
        try {
            const res = await uploadCsvBatch({ ClientId: 1, FileName: file.name, FileChecksum: Checksum })
            if (res.Status === "PROCESSING") {
                setUploading(true);
                Papa.parse<CsvRow>(file, {
                    header: true,
                    skipEmptyLines: true,
                    complete: async (results) => {
                        const validation = validateCsv(results);
                        if (validation.error) {
                            setMessages({ error: validation.error, warning: null, success: null });
                            setRows([]);
                            setUploading(false);
                            setProgress(0);
                            return;
                        }

                        const parsedRows = validateDataConstraints(results.data);
                        const invalidCount = parsedRows.filter(r => !r.IsValid).length;

                        const warningMessage = invalidCount > 0
                            ? "Some records are invalid."
                            : null;

                        setRows(parsedRows);
                        setMessages({ error: null, warning: warningMessage, success: null });

                        if (parsedRows.length > 0) {
                            const payload: CsvData = {
                                BatchId: res.BatchId,
                                FileChecksum: res.FileChecksum,
                                Rows: parsedRows
                            };

                            try {
                                const response = await uploadRegistrations(payload);
                                setMessages(prev => ({
                                    ...prev,
                                    success: `Batch uploaded successfully, View the results below in PROCESSING status with Batch Id - ${response.BatchId}`
                                }));
                            } catch (err) {
                                setMessages(prev => ({
                                    ...prev,
                                    error: (err as Error).message
                                }));
                            }
                        }
                        setUploading(false);
                    }
                });
            }
            if (res.Status === "COMPLETED") {
                setMessages({ error: `Same batch has been submitted previously, View the results below in COMPLETED status with Batch Id - ${res.BatchId}`, warning: null, success: null });
                setRows([]);
                return;
            }
        } catch (err) {
            setMessages({ error: (err as Error).message, warning: null, success: null });
            setRows([]);
            return;
        }

        
    }, []);

    return {
        rows,
        messages,
        uploading,
        progress,
        batchResults,
        handleFileUpload,
        setRows,
        setMessages,
        setBatchResults,
        inProgressResults,
        setInProgressResults
    };
}

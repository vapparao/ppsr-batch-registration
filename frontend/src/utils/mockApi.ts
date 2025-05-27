import type { BatchForm } from '../types/BatchForm';
import type { BatchResult } from '../types/BatchResult';
import type { CsvData } from '../types/CsvData';

const uploadedBatches = new Set<string>();

export async function mockUploadCsvBatch(payload: BatchForm): Promise<BatchResult> {
    await new Promise((res) => setTimeout(res, 300));
    return (uploadedBatches.has(payload.FileChecksum)) ? {
        BatchId: 1,
        ClientId: 1,
        FileName: "sample.csv",
        FileChecksum: payload.FileChecksum,
        Status: "COMPLETED",
        SubmittedAt: new Date(),
        CompletedAt: new Date(),
        TotalRecords: 10,
        ValidRecords: 5,
        InvalidRecords: 5,
        AddedRecords: 3,
        UpdatedRecords: 2
    } : {
        BatchId: 1,
        ClientId: 1,
        FileName: "sample.csv",
        FileChecksum: payload.FileChecksum,
        Status: "PROCESSING",
        SubmittedAt: new Date(),
        CompletedAt: new Date(),
        TotalRecords: 0,
        ValidRecords: 0,
        InvalidRecords: 0,
        AddedRecords: 0,
        UpdatedRecords: 0
    };
}

export async function mockUploadRegistrations(data: CsvData): Promise<BatchResult> {
    await new Promise((res) => setTimeout(res, 500));

    if (data.FileChecksum)
    uploadedBatches.add(data.FileChecksum);

    return {
        BatchId: 1,
        ClientId: 1,
        FileName: "sample.csv",
        FileChecksum: data.FileChecksum,
        Status: "PROCESSING",
        SubmittedAt: new Date(),
        CompletedAt: new Date(),
        TotalRecords: 0,
        ValidRecords: 0,
        InvalidRecords: 0,
        AddedRecords: 0,
        UpdatedRecords: 0
    };
}

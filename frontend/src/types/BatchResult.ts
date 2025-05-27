export interface BatchResult {
    BatchId?: number,
    ClientId?: number,
    FileName?: string,
    FileChecksum?: string,
    Status?: string,
    SubmittedAt?: Date,
    CompletedAt?: Date,
    TotalRecords?: number,
    ValidRecords?: number,
    InvalidRecords?: number,
    AddedRecords?: number,
    UpdatedRecords?: number
}

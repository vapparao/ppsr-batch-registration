import type { CsvRowDto } from "./CsvRowDto";

export interface CsvData {
    BatchId?: number;
    FileChecksum?: string;
    Rows: CsvRowDto[];
}
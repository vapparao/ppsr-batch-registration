import axios from 'axios';
import type { BatchResult } from '../types/BatchResult';
import type { BatchForm } from '../types/BatchForm';

const BASE_URL = 'http://localhost:8080/api/batches';

export async function uploadCsvBatch(payload: BatchForm): Promise<BatchResult> {
    try {
        const response = await axios.post<BatchResult>(`${BASE_URL}/upload`, payload, {
            headers: { 'Content-Type': 'application/json' }
        });
        return response.data;
    } catch (error) {
        console.error('Error uploading CSV batch:', error);
        throw new Error('Server error, problem creating batch');
    }
}

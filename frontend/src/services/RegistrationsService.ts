import axios from 'axios';
import type { BatchResult } from '../types/BatchResult';
import type { CsvData } from '../types/CsvData';

const BASE_URL = 'http://localhost:8080/api/registrations';

export async function uploadRegistrations(payload: CsvData): Promise<BatchResult> {
    try {
        const response = await axios.post<BatchResult>(`${BASE_URL}/process`, payload, {
            headers: { 'Content-Type': 'application/json' }
        });
        return response.data;
    } catch (error) {
        console.error('Error uploading CSV data:', error);
        throw new Error('Server error, problem uploading data for processing');
    }
}

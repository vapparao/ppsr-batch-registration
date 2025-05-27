import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import type { BatchResult } from '../types/BatchResult';

let connection: HubConnection | null = null;
const BASE_URL = 'http://localhost:8080';

export const startConnection = async () => {
    connection = new HubConnectionBuilder()
        .withUrl(`${BASE_URL}/batchHub`)
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();

    try {
        await connection.start();
        console.log("SignalR Connected");
    } catch (err) {
        console.log("Connection failed: ", err);
        setTimeout(startConnection, 5000);
    }
};

export const registerBatchProcessedHandler = (callback: (batchOperations: BatchResult[], processingResults: BatchResult[]) => void) => {
    connection?.on("BatchProcessed", callback);
};

export const stopConnection = async () => {
    if (connection) {
        await connection.stop();
        connection = null;
    }
};

export const getConnectionState = () => {
    return connection?.state;
};
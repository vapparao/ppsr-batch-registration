import React, { useEffect, useState } from 'react';
import {
    Box,
    Typography,
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableRow,
    CircularProgress,
    Paper,
    Tabs,
    Tab
} from '@mui/material';
import {
    startConnection,
    registerBatchProcessedHandler,
    stopConnection
} from '../../services/BatchWebsocketService';
import type { BatchResult } from '../../types/BatchResult';

interface BatchSummaryProps {
    setBatchResults: (results: BatchResult[]) => void;
    batchResults: BatchResult[];
    setInProgressResults: (results: BatchResult[]) => void;
    inProgressResults: BatchResult[];
    uploading: boolean;
}

const BatchSummary: React.FC<BatchSummaryProps> = ({
    setBatchResults,
    batchResults,
    setInProgressResults,
    inProgressResults,
    uploading
}) => {
    const [activeTab, setActiveTab] = useState<'completed' | 'inProgress'>('completed');

    useEffect(() => {
        startConnection();

        const handleBatchProcessed = (
            completedBatches: BatchResult[],
            processingBatches: BatchResult[]
        ) => {
            setInProgressResults(processingBatches);
            setBatchResults(completedBatches);
        };

        registerBatchProcessedHandler(handleBatchProcessed);

        return () => {
            stopConnection();
        };
    }, [setBatchResults, setInProgressResults]);

    const renderTable = (data: BatchResult[], type: 'completed' | 'inProgress') => (
        <Paper elevation={3} sx={{ mt: 2 }}>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>Batch ID</TableCell>
                        <TableCell>File Name</TableCell>
                        <TableCell>Status</TableCell>
                        {type === 'completed' && (
                            <>
                                <TableCell>Total Records</TableCell>
                                <TableCell>Valid Records</TableCell>
                                <TableCell>Invalid Records</TableCell>
                                <TableCell>Added Records</TableCell>
                                <TableCell>Updated Records</TableCell>
                                
                            </>
                        )}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.map((batch) => (
                        <TableRow key={batch.BatchId}>
                            <TableCell>{batch.BatchId}</TableCell>
                            <TableCell>{batch.FileName}</TableCell>
                            <TableCell>{batch.Status}</TableCell>
                            {type === 'completed' && (
                                <>
                                    <TableCell>{batch.TotalRecords}</TableCell>
                                    <TableCell>{batch.ValidRecords}</TableCell>
                                    <TableCell>{batch.InvalidRecords}</TableCell>
                                    <TableCell>{batch.AddedRecords}</TableCell>
                                    <TableCell>{batch.UpdatedRecords}</TableCell>
                                </>
                            )}
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </Paper>
    );

    return (
        <Box sx={{ mt: 4 }}>
            <Typography variant="h5" gutterBottom>
                Batch Processing Summary
            </Typography>

            <Tabs
                value={activeTab}
                onChange={(_, newValue) => setActiveTab(newValue)}
                sx={{ mb: 2 }}
            >
                <Tab label="Completed Batches" value="completed" />
                <Tab label="Processing Batches" value="inProgress" />
            </Tabs>

            {uploading && (
                <Box display="flex" justifyContent="center" my={4}>
                    <CircularProgress />
                    <Typography variant="body1" sx={{ ml: 2 }}>
                        Uploading batch...
                    </Typography>
                </Box>
            )}
            {activeTab === 'inProgress' && (
                <>
                    <Typography variant="h6" gutterBottom>
                        Processing Batches
                    </Typography>
                    {inProgressResults.length > 0 ? (
                        renderTable(inProgressResults, 'inProgress')
                    ) : (
                        <Typography variant="body1" color="textSecondary">
                            No processing batches yet
                        </Typography>
                    )}
                </>
            )}
            {activeTab === 'completed' && (
                <>
                    <Typography variant="h6" gutterBottom>
                        Completed Batches
                    </Typography>
                    {batchResults.length > 0 ? (
                        renderTable(batchResults, 'completed')
                    ) : (
                        <Typography variant="body1" color="textSecondary">
                            No completed batches yet
                        </Typography>
                    )}
                </>
            )}
        </Box>
    );
};

export default BatchSummary;
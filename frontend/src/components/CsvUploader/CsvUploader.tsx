import React, { useRef } from 'react';
import { Box } from '@mui/material';
import { useCsvUpload } from '../../hooks/useCsvUpload';
import DragDropArea from './DragDropArea';
import UploadButton from './UploadButton';
import UploadProgress from './UploadProgress';
import BatchSummary from '../Summary/BatchSummary';
import MessageDisplay from './MessageDisplay';

const CsvUploader: React.FC = () => {
    const fileInputRef = useRef<HTMLInputElement>(null);
    const {
        messages, uploading, progress,
        handleFileUpload, setMessages, setRows, batchResults, setBatchResults, inProgressResults, setInProgressResults
    } = useCsvUpload();

    const handleFileInputChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            await handleFileUpload(file);
            event.target.value = '';
        }
    };


    const onDrop = async (acceptedFiles: File[]) => {
        const file = acceptedFiles[0];
        if (file) await handleFileUpload(file);
    };

    const onDropRejected = () => {
        setMessages(prev => ({
            ...prev,
            error: 'File is too large. Maximum allowed size is 25MB.'
        }));
        setRows([]);
    };

    return (
        <Box sx={{ p: 2 }}>
            <DragDropArea
                onDrop={onDrop}
                onDropRejected={onDropRejected}
                fileInputRef={fileInputRef}
                handleFileInputChange={handleFileInputChange}
            />
            <UploadButton onClick={() => fileInputRef.current?.click()} />
            <UploadProgress uploading={uploading} progress={progress} />
            <MessageDisplay messages={messages} />
            <BatchSummary
                setBatchResults={setBatchResults}
                batchResults={batchResults}
                setInProgressResults={setInProgressResults}
                inProgressResults={inProgressResults}
                uploading={uploading} />
        </Box>
    );
};

export default CsvUploader;

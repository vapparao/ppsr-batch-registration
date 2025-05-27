import React from 'react';
import { Box, Typography } from '@mui/material';
import { useDropzone } from 'react-dropzone';

interface Props {
    onDrop: (files: File[]) => void;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onDropRejected: (rejected: any[]) => void;
    fileInputRef: React.RefObject<HTMLInputElement | null>;
    handleFileInputChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

const DragDropArea: React.FC<Props> = ({
    onDrop,
    onDropRejected,
    fileInputRef,
    handleFileInputChange,
}) => {
    const { getRootProps, getInputProps, isDragActive } = useDropzone({
        onDrop,
        onDropRejected,
        accept: { 'text/csv': ['.csv'] },
        maxFiles: 1,
        maxSize: 25 * 1024 * 1024,
        noClick: true,
    });

    return (
        <>
            <Box {...getRootProps()} sx={{
                border: '2px dashed #ccc',
                p: 4,
                textAlign: 'center',
                backgroundColor: isDragActive ? '#f0f0f0' : 'transparent',
            }}>
                <input {...getInputProps()} />
                <Typography variant="body1">
                    {isDragActive ? 'Drop the CSV file here...' : 'Drag and drop a CSV file here'}
                </Typography>
            </Box>
            <input
                type="file"
                accept=".csv"
                ref={fileInputRef}
                style={{ display: 'none' }}
                onChange={handleFileInputChange}
            />
        </>
    );
};

export default DragDropArea;

import React from 'react';
import { Box, Typography, LinearProgress } from '@mui/material';

const UploadProgress: React.FC<{ uploading: boolean; progress: number }> = ({ uploading, progress }) =>
    uploading ? (
        <Box sx={{ mt: 2 }}>
            <Typography variant="body2">Uploading...</Typography>
            <LinearProgress variant="determinate" value={progress} />
        </Box>
    ) : null;

export default UploadProgress;

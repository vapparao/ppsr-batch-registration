import React from 'react';
import { Box, Button, Typography } from '@mui/material';

const UploadButton: React.FC<{ onClick: () => void }> = ({ onClick }) => (
    <>
        <Box sx={{ mt: 2 }}>
            <Typography variant="body1" align="center">Or</Typography>
        </Box>
        <Box sx={{ mt: 2, textAlign: 'center' }}>
            <Button variant="contained" onClick={onClick}>Upload CSV File</Button>
        </Box>
    </>
);

export default UploadButton;

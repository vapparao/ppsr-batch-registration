import React from 'react';
import { Typography } from '@mui/material';

interface Props {
    messages: {
        error: string | null;
        warning: string | null;
        success: string | null;
    };
}

const MessageDisplay: React.FC<Props> = ({ messages }) => (
    <>
        {messages.error && <Typography color="error" sx={{ mt: 2 }}>{messages.error}</Typography>}
        {messages.warning && <Typography color="warning.main" sx={{ mt: 2 }}>{messages.warning}</Typography>}
        {messages.success && <Typography color="success.main" sx={{ mt: 2 }}>{messages.success}</Typography>}
    </>
);

export default MessageDisplay;

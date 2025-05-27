import CsvUploader from './components/CsvUploader/CsvUploader';
import { Box, Grid, Paper } from '@mui/material';

function App() {
    return (
        <Grid container height="100vh">
            <Grid size={12} bgcolor="primary.main" textAlign="center" height="10vh" position="relative">
                <h2>Batch create motor vehicle PPSR registrations</h2>
            </Grid>
            <Grid size={12} textAlign="center" height="90vh" >
                <Paper elevation={24} >
                    <Box sx={{ p: 2, m: 2, textAlign: "center", margin: "auto", width: "98vw" }}>
                        <CsvUploader />
                    </Box>
                </Paper>
            </Grid>
            
        </Grid>
        
        
    );
}

export default App;

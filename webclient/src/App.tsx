import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import HomePage from "./Pages/HomePage";
import LoginPage from "./Pages/LoginPage";
import PatientsPage from "./Pages/PatientsPage";
import SchedulePage from "./Pages/SchedulePage";
import PatientDetailsPage from "./Pages/PatientDetailsPage";
import MainLayout from "./layouts/MainLayout";
import ErrorBoundary from "./ErrorHandlingMiddleware/ErrorBoundary";


const App: React.FC = () => {
    return (
        <ErrorBoundary>
            <Router>
                <Routes>
                    {/* Логин отдельный, без header */}
                    <Route path="/" element={<LoginPage />} />

                    {/* Все страницы под layout с header */}
                    <Route element={<MainLayout />}>
                        <Route path="/home" element={<HomePage />} />
                        <Route path="/patients" element={<PatientsPage />} />
                        <Route path="/schedule" element={<SchedulePage />} />
                        <Route path="/patients/:id" element={<PatientDetailsPage />} />

                    </Route>
                </Routes>
            </Router>
        </ErrorBoundary>
    );
};

export default App;

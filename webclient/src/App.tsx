import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./Context/AuthContext";
import { ToastProvider } from "./Hooks/useToast";
import LoginPage from "./Pages/LoginPage";
import AnalyticsDashboardPage from "./Pages/AnalyticsDashboardPage";
import PatientDetailsPage from "./Pages/PatientDetailsPage";
import HomePage from "./Pages/HomePage";
import PatientsPage from "./Pages/PatientsPage";
import SchedulePage from "./Pages/SchedulePage";
import UsersPage from "./Pages/UsersPage";
import ReferencePage from "./Pages/ReferencePage";
import DoctorsPage from "./Pages/DoctorsPage";
import { DashboardLayout } from "./Components/Layout/DashboardLayout";
import ErrorBoundary from "./ErrorHandlingMiddleware/ErrorBoundary";

const App: React.FC = () => {
    return (
        <AuthProvider>
        <ToastProvider>
        <ErrorBoundary>
            <Router>
                <Routes>
                    <Route path="/" element={<LoginPage />} />
                    <Route element={<DashboardLayout />}>
                        <Route path="/home"          element={<HomePage />} />
                        <Route path="/patients"       element={<PatientsPage />} />
                        <Route path="/patients/:id"   element={<PatientDetailsPage />} />
                        <Route path="/schedule"       element={<SchedulePage />} />
                        <Route path="/analitics"      element={<AnalyticsDashboardPage />} />
                        <Route path="/users"          element={<UsersPage />} />
                        <Route path="/reference"      element={<ReferencePage />} />
                        <Route path="/doctors"        element={<DoctorsPage />} />
                    </Route>
                </Routes>
            </Router>
        </ErrorBoundary>
        </ToastProvider>
        </AuthProvider>
    );
};

export default App;

import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import HomePage from "./pages/HomePage";
import PatientsPage from "./pages/PatientsPage";
import SchedulePage from "./pages/SchedulePage";
import MainLayout from "./layouts/MainLayout";
import PatientForm from "./pages/PatientForm";


const App: React.FC = () => {
    return (
        <Router>
            <Routes>
                {/* Логин отдельный, без header */}
                <Route path="/" element={<LoginPage />} />

                {/* Все страницы под layout с header */}
                <Route element={<MainLayout />}>
                    <Route path="/home" element={<HomePage />} />
                    <Route path="/patients" element={<PatientsPage />} />
                    <Route path="/schedule" element={<SchedulePage />} />
                    <Route path="/patients/create" element={<PatientForm />} />
                    <Route path="/patients/:id/edit" element={<PatientForm />} />
                </Route>
            </Routes>
        </Router>
    );
};

export default App;

import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import HomePage from "./Pages/HomePage";
import LoginPage from "./Pages/LoginPage";
import MainLayout from "./Layouts/MainLayout";
import PatientForm from "./Pages/PatientForm";
import PatientsPage from "./Pages/PatientsPage";
import SchedulePage from "./Pages/SchedulePage";
import PatientDetailsPage from "./Pages/PatientDetailsPage";


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
                    <Route path="/patients/:id" element={<PatientDetailsPage />} />

                </Route>
            </Routes>
        </Router>
    );
};

export default App;

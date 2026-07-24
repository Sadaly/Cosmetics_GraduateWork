// models.ts

/** Перечисления */
export type ReservedDateType =
    | 'None'
    | 'TimeRestrict'
    | 'DayOfWeekRestrict'
    | 'HolidayRestrict';

/** Модели ответов от сервера */
export interface Doctor {
    id: string;
    name: string;
}

export interface ExternalProcedureRecord {
    patientCardId: string;
    typeId: string;
    date: string; // ISO 8601
    typeName: string;
}

export interface AgeChange {
    patientCardId: string;
    typeId: string;
    typeName: string;
}

export interface AgeChangeType {
    id: string;
    title: string;
}

export interface HealthCond {
    patientCardId: string;
    typeId: string;
    typeName: string;
}

export interface HealthCondType {
    id: string;
    title: string;
}

export interface Notification {
    id: string;
    procedureId: string;
    message: string;
    phoneNumber: string;
}

export interface PatientCard {
    id: string;
    patientId: string;
    fullname: string;
    age: number;
    address: string;
    complaints: string;
    phoneNumber: string;
}

export interface PatientSpecifics {
    id: string;
    patientCardId: string;
    sleep: string;
    diet: string;
    sport: string;
    workEnviroment: string; // Сохранена оригинальная опечатка из C# модели
}

export interface Patient {
    patientId: string;
    fullname: string;
    cardId: string;
    address: string;
    age: number;
    complaints: string;
    phoneNumber: string;
}

export interface ExternalProcedureRecordType {
    id: string;
    title: string;
}

export interface Procedure {
    procedureId: string;
    patientCardId: string;
    price: number;
    scheduledDate: string | null; // ISO 8601
    typeId: string;
    title: string;
    doctorId: string | null;
}

export interface ProcedureType {
    id: string;
    title: string;
    price: number;       // StandartPrice from backend
    duration: number;    // StandartDuration from backend
}

export interface ReservedDate {
    startDate: string; // ISO 8601
    endDate: string;   // ISO 8601
    type: ReservedDateType;
}

export interface SkinCare {
    patientCardId: string;
    typeId: string;
    typeName: string;
}

export interface SkinCareType {
    id: string;
    title: string;
}

export interface SkinFeature {
    patientCardId: string;
    typeId: string;
    typeName: string;
}

export interface SkinFeatureType {
    id: string;
    title: string;
}

export interface User {
    userId: string;
    username: string;
    email: string;
    registrationDate: string; // ISO 8601
    updateDate: string;       // ISO 8601
}
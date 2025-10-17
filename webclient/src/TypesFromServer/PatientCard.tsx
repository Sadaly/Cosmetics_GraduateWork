export interface PatientCard {
    id: string;          // maps to Id (Guid)
    patientId: string;   // maps to PatientId (Guid)
    fullname: string;    // maps to Fullname
    age: number;         // maps to Age (byte)
    address: string;     // maps to Address
    complaints: string;  // maps to Complaints
    phoneNumber: string; // maps to PhoneNumber
}

import { server } from "typescript"

export interface Register{
    name: string,
    email: string,
    password: string,
    confirmPassword: string,
    gender:string,
    dateOfBirth: string | undefined
}
export interface User {
  id: string;
  name: string;
  email: string;
  phone: string;
  roleId: number;      
  isBlocked?: boolean;
}
export enum Roles {
  Admin = 1,
  User = 2
}

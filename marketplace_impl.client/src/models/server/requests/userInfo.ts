export interface User {
  id: number;
  name: string;
  phone: string;
  isRegisteredSeller: boolean;
}

export default interface UserInfo {
  isSignedIn: boolean;
  info?: User;
}

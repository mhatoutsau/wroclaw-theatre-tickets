import React, { createContext, useContext, useState, useEffect } from "react";
import { apiClient } from "../api/client";
import type {
  AuthenticationResponse,
  UserLoginRequest,
  UserRegistrationRequest,
} from "../types/api";

interface User {
  id: string;
  email: string;
  role: "User" | "Moderator" | "Admin";
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (credentials: UserLoginRequest) => Promise<void>;
  register: (data: UserRegistrationRequest) => Promise<void>;
  logout: () => void;
  isAdmin: boolean;
  isModerator: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check if user is already logged in
    const token = apiClient.getToken();
    if (token) {
      // Decode JWT to get user info
      try {
        const payload = JSON.parse(atob(token.split(".")[1]));
        // ClaimTypes.Role in .NET is mapped to this long claim name in JWT
        const role =
          payload[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ] ||
          payload.role ||
          "User";
        const nameIdentifier =
          payload[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/nameidentifier"
          ] ||
          payload.sub ||
          payload.nameid;
        const email =
          payload[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
          ] || payload.email;

        setUser({
          id: nameIdentifier,
          email: email,
          role: role as "User" | "Moderator" | "Admin",
        });
      } catch {
        apiClient.clearToken();
      }
    }
    setIsLoading(false);
  }, []);

  const login = async (credentials: UserLoginRequest) => {
    const response: AuthenticationResponse = await apiClient.login(credentials);
    apiClient.setToken(response.accessToken);

    // Decode JWT to get user info
    const payload = JSON.parse(atob(response.accessToken.split(".")[1]));
    const role =
      payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
      payload.role ||
      "User";

    setUser({
      id: response.userId,
      email: response.email,
      role: role as "User" | "Moderator" | "Admin",
    });
  };

  const register = async (data: UserRegistrationRequest) => {
    const response: AuthenticationResponse = await apiClient.register(data);
    apiClient.setToken(response.accessToken);

    // Decode JWT to get user info
    const payload = JSON.parse(atob(response.accessToken.split(".")[1]));
    const role =
      payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
      payload.role ||
      "User";

    setUser({
      id: response.userId,
      email: response.email,
      role: role as "User" | "Moderator" | "Admin",
    });
  };

  const logout = () => {
    apiClient.clearToken();
    setUser(null);
  };

  const isAdmin = user?.role === "Admin";
  const isModerator = user?.role === "Moderator" || isAdmin;

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        isLoading,
        login,
        register,
        logout,
        isAdmin,
        isModerator,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}

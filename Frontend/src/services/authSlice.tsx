// src/core/data/redux/slices/authSlice.ts
import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "./store";

interface User {
  userId: number | null;
  userFullName: string | null;
  userRoleName: string | null;
}

interface AuthState {
  token: string | null;
  user: User | null;
  isAuthenticated: boolean;
}

const initialState: AuthState = {
  token: null,
  user: null,
  isAuthenticated: false,
};

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    loginSuccess: (
      state,
      action: PayloadAction<{
        authtoken: string;
        user: User;
      }>
    ) => {
      state.token = action.payload.authtoken;
      state.user = action.payload.user;
      state.isAuthenticated = true;
      localStorage.setItem("authToken", action.payload.authtoken);
      localStorage.setItem(
        "userId",
        action.payload.user.userId?.toString() || ""
      );
    },
    logout: (state) => {
      state.token = null;
      state.user = null;
      state.isAuthenticated = false;
      localStorage.removeItem("authToken");
      localStorage.removeItem("userId");
    },
    setTokenFromStorage: (state) => {
      const token = localStorage.getItem("authToken");
      if (token) {
        state.token = token;
        state.isAuthenticated = true;
      }
    },
    refreshTokenSuccess: (state, action: PayloadAction<string>) => {
      state.token = action.payload;
      state.isAuthenticated = true;
      localStorage.setItem("authToken", action.payload);
    },
  },
});

export const {
  loginSuccess,
  logout,
  setTokenFromStorage,
  refreshTokenSuccess,
} = authSlice.actions;
export const selectIsAuthenticated = (state: RootState) =>
  state.auth.isAuthenticated;

export const loggedUser = {
  user: (state: RootState) => state.auth.user,
};

export default authSlice.reducer;

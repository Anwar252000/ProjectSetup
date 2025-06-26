import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import type { ApiResponse } from "./apiResponse";

export interface UserData {
  userId?: number;
  userName?: string;
  token?: string;
  passwordHash?: string;
  roleId?: number;
  roleName?: string;
  fullName?: string;
  createdAt?: Date;
}

export const authApi = createApi({
  reducerPath: "authApi",
  baseQuery: fetchBaseQuery({
    baseUrl: `${import.meta.env.VITE_BASE_API_URL}Auth/`,
  }),
  endpoints: (builder) => ({
    login: builder.mutation<ApiResponse<UserData>, UserData>({
      query: (credentials) => ({
        url: "Login",
        method: "POST",
        body: credentials,
      }),
    }),

    addUser: builder.mutation<ApiResponse<UserData>, UserData>({
      query: (newUser) => ({
        url: "AddUser",
        method: "POST",
        body: newUser,
      }),
    }),
    getAllUsers: builder.query<ApiResponse<UserData[]>, void>({
      query: () => "GetAllUsers",
    }),

    getUserById: builder.query<ApiResponse<UserData>, number>({
      query: (id) => `GetUserById?userId=${id}`,
    }),

    updateUser: builder.mutation<ApiResponse<void>, UserData>({
      query: (userData) => ({
        url: "UpdateUser",
        method: "PUT",
        body: userData,
      }),
    }),

    deleteUser: builder.mutation<ApiResponse<void>, number>({
      query: (id) => ({
        url: `DeleteUser?userId=${id}`,
        method: "DELETE",
      }),
    }),
  }),
});

export const {
  useLoginMutation,
  useAddUserMutation,
  useDeleteUserMutation,
  useUpdateUserMutation,
  useGetAllUsersQuery,
  useGetUserByIdQuery,
} = authApi;

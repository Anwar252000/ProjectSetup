// src/core/data/redux/store.ts
import { configureStore } from "@reduxjs/toolkit";

import { persistReducer, persistStore } from "redux-persist";
import storage from "redux-persist/lib/storage";
import { combineReducers } from "redux";
import { authApi } from "./apis/authApi";
import authSlice from "./authSlice";
import permissionSlice from "./userPermissionSlice";

// Create persist config for the auth slice
const authPersistConfig = {
  key: "auth", // Key used for localStorage
  storage, // The storage engine to use (localStorage)
};

const permissionPersistConfig = {
  key: "permission", // Key for localStorage
  storage, // Storage engine to use (localStorage)
};
// Combine reducers for persist config
const rootReducer = combineReducers({
  permission: persistReducer(permissionPersistConfig, permissionSlice),
  auth: persistReducer(authPersistConfig, authSlice), // Persisted auth slice
  [authApi.reducerPath]: authApi.reducer,
});

// Configure the store
const store = configureStore({
  reducer: rootReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false, // Disable serializable checks for redux-persist
    }).concat(authApi.middleware),
});

// Create a persistor to manage the persisted state
export const persistor = persistStore(store);

export type RootState = ReturnType<typeof store.getState>;
export default store;

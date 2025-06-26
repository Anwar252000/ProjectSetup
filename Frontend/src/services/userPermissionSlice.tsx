import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface PermissionState {
  permissions: string[]; // Store modules as a list of strings
}

const initialState: PermissionState = {
  permissions: [],
};

const permissionSlice = createSlice({
  name: "permission",
  initialState,
  reducers: {
    setPermissions: (state, action: PayloadAction<string[]>) => {
      state.permissions = action.payload;
    },
    clearPermissions: (state) => {
      state.permissions = [];
    },
  },
});

export const { setPermissions, clearPermissions } = permissionSlice.actions;
export default permissionSlice.reducer;

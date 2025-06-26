import React from "react";
import { useGetAllUsersQuery, type UserData } from "../services/apis/authApi";

const Home: React.FC = () => {
  const { data: users } = useGetAllUsersQuery();
  const userData = users?.data as UserData[];
  return (
    <>
      <h1>Home</h1>
      {userData?.map((user) => (
        <div key={user.userId}>
          <p>{user.fullName}</p>
          <p>{user.roleName}</p>
        </div>
      ))}
    </>
  );
};

export default Home;

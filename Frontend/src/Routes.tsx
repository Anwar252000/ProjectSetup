// import React from "react";

import { BrowserRouter, Route, Routes } from "react-router-dom";
import Home from "./routes/Home";
import Wrapper from "./components/Wrapper";
import About from "./routes/About";

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Wrapper />}>
          <Route index element={<Home />} />
          <Route path="about" element={<About />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default App;

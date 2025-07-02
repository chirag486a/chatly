import React, { useCallback, useContext } from "react";
import AuthContext from "../Context/AuthContext";
import axios from "axios";

const API_ROUTE = `https://localhost:44347/api/accounts`;

function isPlainObjectStrict(obj) {
  if (typeof obj !== "object" || obj === null) return false;
  return Object.getPrototypeOf(obj) === Object.prototype;
}

function AuthProvider({ children }) {
  async function login(credentials) {
    try {
      if (
        credentials === null ||
        credentials === undefined ||
        !isPlainObjectStrict(credentials)
      ) {
        throw new Error(
          "Credentials cannot be null, undefined or non-object value"
        );
      }
      if (
        credentials.Email === null ||
        credentials.Email === undefined ||
        credentials.Email.length === 0
      ) {
        throw new Error("Email cannot be null, undefined or empty");
      }
      if (
        credentials.Password === null ||
        credentials.Password === undefined ||
        credentials.Password.length === 0
      ) {
        throw new Error("Email cannot be null, undefined or empty");
      }

      let message = await axios.post(`${API_ROUTE}/login`, credentials, {
        headers: {
          "Content-Type": "application/json",
        },
      });
      if (!isPlainObjectStrict(message.data)) {
        throw "Something went wrong";
      }
      return message.data;
    } catch (e) {
      let error = e.response.data;
      console.error(error);
      throw error;
    }
  }

  return (
    <AuthContext.Provider value={{ login }}>{children}</AuthContext.Provider>
  );
}

export default AuthProvider;

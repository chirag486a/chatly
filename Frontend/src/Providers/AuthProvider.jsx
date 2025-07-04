import AuthContext from "../Context/AuthContext";
import axios from "axios";
import ValidationError from "../Exceptions/ValidationError";
import NetworkError from "../Exceptions/NetworkError";
import BadRequest from "../Exceptions/BadRequest";
import ArgumentError from "../Exceptions/ArgumentError";
import AuthenticationError from "../Exceptions/AuthenticationError";
import UserNotFound from "../Exceptions/UserNotFound";

const API_ROUTE = `http://localhost:5280/api/accounts`;

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
        throw new ValidationError(
          "Email cannot be null, undefined or empty",
          "EMAIL"
        );
      }
      if (
        credentials.Password === null ||
        credentials.Password === undefined ||
        credentials.Password.length === 0
      ) {
        throw new ValidationError(
          "Password cannot be null, undefined or empty",
          "PASSWORD"
        );
      }
      console.log(credentials);

      let message = await axios.post(`${API_ROUTE}/login`, credentials, {
        headers: {
          "Content-Type": "application/json",
        },
      });
      console.log(message);
      return message.data;
    } catch (e) {
      if (e.code === "ERR_NETWORK") {
        throw new NetworkError(e.message, e);
      }
      if (e.code == "ERR_BAD_REQUEST") {
        throw new BadRequest(e?.response?.data?.message);
      }
      throw e;
    }
  }
  async function signup(formData = {}) {
    try {
      let message = await axios.post(`${API_ROUTE}/signup`, formData, {
        headers: {
          "Content-Type": "application/json",
        },
      });
      return message.data;
    } catch (e) {
      console.error(e);
      if (e.code === "ERR_NETWORK") {
        throw new NetworkError(e.message, e);
      }
      if (e.code === "ERR_BAD_REQUEST") {
        throw new BadRequest(e?.response?.data?.message, e?.response?.data);
      }
      throw e;
    }
  }
  function saveToken(token) {
    if (!token) {
      throw new ArgumentError("Invalid token");
    }
    localStorage.setItem("token", token);
  }
  function getToken() {
    let token = localStorage.getToken("token");
    if (!token) {
      throw new AuthenticationError("Could not find the token");
    }
    return token;
  }
  function saveUser(user) {
    localStorage.setItem("user", JSON.stringify(user));
  }
  function getUser() {
    try {
      var stringUser = localStorage.setItem("user");
      if (stringUser) {
        throw new UserNotFound("User not found in the localstorage");
      }
      return JSON.parse(stringUser);
    } catch (e) {
      throw new UserNotFound(
        "Couldn't parse the user from localstorage",
        null,
        e
      );
    }
  }

  return (
    <AuthContext.Provider
      value={{ login, saveToken, getToken, saveUser, getUser, signup }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export default AuthProvider;

import AuthContext from "../Context/AuthContext";
import axios from "axios";
import ValidationError from "../Exceptions/ValidationError";
import NetworkError from "../Exceptions/NetworkError";
import BadRequest from "../Exceptions/BadRequest";

const API_ROUTE = `https://localhost:7174/api/accounts`;

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
      console.log(message)
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
  function saveToken(token) {
    localStorage.setItem("token", token);
  }
  function getToken() {
    return localStorage.getToken("token");
  }
  function saveUser(user) {
    localStorage.setItem("user", JSON.stringify(user));
  }
  function getUser() {
    localStorage.setItem("user");
  }

  return (
    <AuthContext.Provider
      value={{ login, saveToken, getToken, saveUser, getUser }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export default AuthProvider;

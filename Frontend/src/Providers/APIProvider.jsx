"use strict";
import axios from "axios";
import APIContext from "../Context/APIContext";
import NetworkError from "../Exceptions/NetworkError";
import BadRequest from "../Exceptions/BadRequest";
import AuthenticationError from "../Exceptions/AuthenticationError";
import { useCallback, useState } from "react";

let API_ROUTE = "http://localhost:5280/api";

export default function APIProvider({ children }) {
  const [contacts, setContacts] = useState([]);
  async function searchUsers(query = "", page = 1, pageSize = 5, token = "") {
    try {
      let route = `${API_ROUTE}/Users/Search`;
      let response = await axios.get(route, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
        param: {
          Query: query,
          Page: page,
          PageSize: pageSize,
        },
      });
      return response.data;
    } catch (e) {
      console.log(e);
      if (e.code === "ERR_NETWORK") throw new NetworkError(e.message, { e });
      if (e.status === 401)
        throw new AuthenticationError("User not Authenticated");

      if (e.code === "ERR_BAD_REQUEST") {
        throw new BadRequest(e.message, e?.response, { e });
      }
      throw e;
    }
  }
  const loadContacts = useCallback(async function (
    page = 1,
    pageSize = 10,
    token = ""
  ) {
    try {
      const route = `${API_ROUTE}/contacts/`;
      const response = await axios.get(route, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
        param: {
          Page: page,
          PageSize: pageSize,
        },
      });
      setContacts([...response.data.data]);
    } catch (e) {
      console.error(e);
    }
  },
  []);
  return (
    <APIContext.Provider value={{ searchUsers, loadContacts, contacts }}>
      {children}
    </APIContext.Provider>
  );
}

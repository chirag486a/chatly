import { useContext, useState } from "react";
import Search from "../../../assets/search.svg?react";
import ChatWindow from "./ChatsWindow";
import ContactLists from "./Components/ContactLists";
import APIContext from "../../../Context/APIContext";
import AuthContext from "../../../Context/AuthContext";
import BadRequest from "../../../Exceptions/BadRequest";
import { Route, Routes, useNavigate, useParams } from "react-router-dom";
import AuthenticationError from "../../../Exceptions/AuthenticationError";
import NoChatSelection from "./Components/NoChatSelection";
export default function ChatEnvironment() {
  const navigate = useNavigate();
  const { searchUsers, currentContact, contacts } = useContext(APIContext);
  const { getToken } = useContext(AuthContext);
  const [searchMode, setSearchMode] = useState(false);
  const [_, setSearch] = useState("");
  console.log(currentContact);
  const { chatId } = useParams();

  async function handleSearch(e) {
    try {
      setSearch(e.target.value);
      setSearchMode(true);
      const response = await searchUsers(e.target.value, 1, 5, getToken());
      console.log(response);
    } catch (e) {
      if (e instanceof AuthenticationError) {
        console.log(`AuthenticationError`);
        navigate("/login");
      }
      if (e instanceof BadRequest) {
        if (e.data.status === 401) {
          navigate("/login");
        }
      }
    }
  }
  const noContacts = contacts.length === 0;
  const noChatSelection = !chatId;
  return (
    <div className="px-6 py-5 flex justify-between">
      <div className="flex flex-col min-w-80 w-full gap-8 flex-1/5 max-w-1/4">
        <div>
          <label className="input opacity-50 rounded-xl px-4 py-2 h-fit w-full space-x-0.5">
            <Search className="h-6" />
            <input
              type="search"
              required
              placeholder="Search users..."
              className="bg-transparent  text-base placeholder-base-content"
              onChange={(e) => handleSearch(e)}
            />
          </label>
        </div>
        <div>
          {!searchMode && <ContactLists />}
          {searchMode && <ContactLists />}
        </div>
      </div>
      <div className="flex-grow">
        <ChatWindow noContacts={noContacts} noChatSelection={noChatSelection} />
      </div>
    </div>
  );
}

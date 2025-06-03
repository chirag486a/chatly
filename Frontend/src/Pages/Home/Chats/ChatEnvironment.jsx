import Search from "../../../assets/search.svg?react";
import ChatWindow from "./ChatsWindow";
import ContactLists from "./Components/ContactLists";
export default function ChatEnvironment() {
  return (
    <div className="px-6 py-5 flex justify-between">
      <div className="flex flex-col min-w-80 w-full gap-8 flex-1/5">
        <div>
          <label className="input opacity-50 rounded-xl px-4 py-2 h-fit w-full space-x-0.5">
            <Search className="h-6" />
            <input
              type="search"
              required
              placeholder="Search users..."
              className="bg-transparent  text-base placeholder-base-content"
            />
          </label>
        </div>
        <div>
          <ContactLists />
        </div>
      </div>
      <div className="flex-grow">
        <ChatWindow noChatSelection={false} noContacts={false} />
      </div>
    </div>
  );
}

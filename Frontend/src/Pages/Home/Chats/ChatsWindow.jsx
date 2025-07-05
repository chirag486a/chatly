import MessageInputField from "./Components/MessageInputField";
import NoChatSelection from "./Components/NoChatSelection";
import ChatWindowTitle from "./Components/ChatWindowTitle";
import ChatsWindowBody from "./Components/ChatsWindowBody";
import { useContext } from "react";
import APIContext from "../../../Context/APIContext";
import { useParams } from "react-router-dom";

export default function ChatsWindow() {
  const { contacts } = useContext(APIContext);
  const { chatId, userId } = useParams();

  if (contacts?.length === 0) {
    return <NoChatSelection noContacts={true} />;
  }

  if (!chatId && !userId) {
    return <NoChatSelection noContacts={false} />;
  }

  if (userId) {
    return (
      <div className="h-full w-[926px]">
        <div className="flex flex-col px-6 h-full gap-6">
          <ChatWindowTitle />
          <div className="flex-grow h-full">
          </div>
          <MessageInputField />
        </div>
      </div>
    );
  }

  return (
    <div className="h-full w-[926px]">
      <div className="flex flex-col px-6 h-full gap-6">
        <ChatWindowTitle />
        <div className="flex-grow h-full">
          <ChatsWindowBody />
        </div>
        <MessageInputField />
      </div>
    </div>
  );
}

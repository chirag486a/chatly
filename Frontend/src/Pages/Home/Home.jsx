import { Route, Routes } from "react-router-dom";
import ChatEnvironment from "./Chats/ChatEnvironment";
import Settings from "./Settings/Settings";
import NotFound from "../NotFound";
import APIProvider from "../../Providers/APIProvider";

export default function Home() {
  return (
    <APIProvider>
      <div>
        <Routes>
          <Route path="/chat/:chatId" element={<ChatEnvironment />} />
          <Route path="/chat/" element={<ChatEnvironment />} />
          <Route path="settings" element={<Settings />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </div>
    </APIProvider>
  );
}

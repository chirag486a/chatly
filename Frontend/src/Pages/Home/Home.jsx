import { Route, Routes } from "react-router-dom";
import ChatEnvironment from "./Chats/ChatEnvironment";
import Settings from "./Settings/Settings";
import NotFound from "../NotFound";
import APIProvider from "../../Providers/APIProvider";
import TopBar from "../../Components/TopBar";

export default function Home() {
  return (
    <APIProvider>
      <div className="flex flex-col">
        <TopBar showProfile={true} />
        <div className="divider m-0"></div>
        <div className="flex">
          <Routes>
            <Route path="/chat/:chatId" element={<ChatEnvironment />} />
            <Route path="/chat/draft/:userId" element={<ChatEnvironment />} />
            <Route path="/chat/" element={<ChatEnvironment />} />
            <Route path="settings" element={<Settings />} />
            <Route path="*" element={<NotFound />} />
          </Routes>
        </div>
      </div>
    </APIProvider>
  );
}

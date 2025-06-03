import { Route, Routes } from "react-router-dom";
import ChatEnvironment from "./Chats/ChatEnvironment";
import Settings from "./Settings/Settings";
import NotFound from "../NotFound";

export default function Home() {
  return (
    <div>
      <Routes>
        <Route path="/" element={<ChatEnvironment />} />
        <Route path="settings" element={<Settings />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </div>
  );
}

import { Link } from "react-router-dom";
import Logo from "../assets/logo.svg?react";
import DefaultUserProfile from "../assets/default-user-profile.svg?react";
import SettingsIcon from "../assets/settings-icon.svg?react";
import LogoutIcon from "../assets/logout-icon.svg?react";
import { useRef, useState } from "react";

export default function TopBar({ showProfile }) {
  const profileBtnRef = useRef(null);
  const [contextMenu, setContextMenu] = useState({
    visible: false, // Should the menu be shown?
    x: 0, // X position on screen
    y: 0, // Y position on screen
  });
  function handleContextMenu(e) {
    if (e.target.closest("#profile-btn") !== null) {
      setContextMenu({
        visible: true,
        x: e.pageX,
        y: e.pageY,
      });
    }
  }
  return (
    <div
      className="w-full bg-base py-3 px-10 flex justify-between"
      onClick={() => {
        if (contextMenu.visible === true) {
          setContextMenu({ ...contextMenu, visible: false });
        }
      }}
    >
      <Link to="/chat" className="flex items-center justify-center">
        <div className="flex items-center gap-3">
          <Logo className="fill-base-content h-6" />
          <p className="text-2xl font-normal text-base-content">Chatly</p>
        </div>
      </Link>
      <div
        className={"rounded-full bg-black h-10 relative cursor-pointer"}
        id="profile-btn"
        onClick={(e) => handleContextMenu(e)}
        ref={profileBtnRef}
      >
        <DefaultUserProfile className={showProfile ? "block" : "hidden"} />
        {contextMenu.visible && (
          <div
            className="absolute bg-neutral-200 right-1/2 rounded shadow-md z-50"
            // style={{ top: contextMenu.y, left: contextMenu.x }}
            style={{
              right: Math.min(window.innerWidth - profileBtnRef.right, 0),
              top: contextMenu.y,
            }}
          >
            <ul className="text-sm w-fit">
              <li className="hover:bg-gray-100 pr-5 flex items-center">
                <Link to="/settings" className="flex items-center">
                  <span className="px-4 py-3 flex items-center justify-center ">
                    <SettingsIcon className="h-4 w-4 fill-base-content" />
                  </span>
                  <p>Settings</p>
                </Link>
              </li>
              <li className="hover:bg-gray-100 pr-5">
                <Link to="/login" className="flex items-center">
                  <span className="px-4 py-3 flex items-center justify-center">
                    <LogoutIcon className="h-4 w-4 fill-base-content" />
                  </span>
                  <p>Log out</p>
                </Link>
              </li>
            </ul>
          </div>
        )}
      </div>
    </div>
  );
}

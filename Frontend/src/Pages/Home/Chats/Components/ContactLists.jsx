import NoContactDisplay from "./NoContactDisplay";
import MailOpen from "../../../../assets/mail-open.svg?react";
import Delete from "../../../../assets/delete-icon.svg?react";
import History from "../../../../assets/history.svg?react";
import Mute from "../../../../assets/mute-icon.svg?react";
import Pin from "../../../../assets/pin-icon.svg?react";
import Archive from "../../../../assets/archive-icon.svg?react";
import Window from "../../../../assets/window-icon.svg?react";
import Contact from "./Contact";
import { useContext, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import APIContext from "../../../../Context/APIContext";
import AuthContext from "../../../../Context/AuthContext";
import AuthenticationError from "../../../../Exceptions/AuthenticationError";

export default function ContactLists() {
  /*
  CONTACT PERSON ID
  CONTACT PERSON PROFILE PICTURE

  CONTACT PERSON LAST CHAT
  CONTACT PERSON LAST CHAT TIME
  */
  const { loadContacts, contacts } = useContext(APIContext);
  const { getToken, getUser } = useContext(AuthContext);
  let user = getUser();
  const navigate = useNavigate();

  useEffect(() => {
    (async function () {
      try {
        loadContacts(1, 10, getToken());
      } catch (e) {
        if (e instanceof AuthenticationError) {
          navigate("/login");
        }
      }
    })();
  }, [loadContacts, getToken, navigate]);

  const [contextMenu, setContextMenu] = useState({
    visible: false, // Should the menu be shown?
    x: 0, // X position on screen
    y: 0, // Y position on screen
    messageId: null, // Which message was clicked?
  });
  const containerRef = useRef(null);
  return (
    <div>
      {contacts?.length === 0 && <NoContactDisplay />}
      {contacts?.length !== 0 && (
        <div
          className="max-h-[512px] overflow-y-scroll"
          ref={containerRef}
          onContextMenu={(e) => {
            e.preventDefault();
            if (e.target.closest(".contact") !== null) {
              if (contextMenu.visible === true) {
                setContextMenu({ visible: false, x: 0, y: 0 });
                return;
              }
              setContextMenu({ visible: true, x: e.pageX, y: e.pageY });
            }
          }}
          onClick={(e) => {
            e.preventDefault();
            if (contextMenu.visible === true) {
              setContextMenu({ ...contextMenu, visible: false });
              return;
            }
          }}
        >
          {contacts.map((data) => {
            let contactUser;
            if (data.contactId == user.id) {
              contactUser = data.user;
            } else contactUser = data.contactUser;

            return (
              <Contact
                isActive={true}
                key={data.id}
                contactId={data.id}
                contactName={contactUser.displayName}
              />
            );
          })}

          {contextMenu.visible && (
            <div
              className="absolute bg-neutral-200 rounded shadow-md z-50"
              style={{ top: contextMenu.y, left: contextMenu.x }}
            >
              <ul className="text-sm">
                <li className="hover:bg-gray-100 cursor-pointer flex items-center">
                  <span className="px-4 py-3 flex items-center justify-center">
                    <Window className="h-4 w-4" />
                  </span>
                  <p>Open in new tab</p>
                </li>
                <li className="hover:bg-gray-100 cursor-pointer flex items-center pr-6">
                  <span className="px-4 py-3 flex items-center justify-center">
                    <Archive className="h-4 w-4" />
                  </span>
                  <p>Archive</p>
                </li>
                <li className="hover:bg-gray-100 cursor-pointer flex items-center pr-6">
                  <span className="px-4 py-3 flex items-center justify-center">
                    <Pin className="h-4 w-4" />
                  </span>
                  <p>Pin</p>
                </li>
                <li className="hover:bg-gray-100 cursor-pointer flex items-center pr-6">
                  <span className="px-4 py-3 flex items-center justify-center">
                    <MailOpen className="h-4 w-4" />
                  </span>
                  <p>Mark as unread</p>
                </li>
                <li className="hover:bg-gray-100 cursor-pointer flex items-center pr-6">
                  <span className="px-4 py-3 flex items-center justify-center">
                    <History className="h-4 w-4" />
                  </span>
                  <p>Clear History</p>
                </li>
                <li className="hover:bg-gray-100 cursor-pointer flex items-center pr-6">
                  <span className="px-4 py-3 flex items-center justify-center">
                    <Delete className="h-4 w-4" />
                  </span>
                  <p>Delete chat</p>
                </li>
              </ul>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

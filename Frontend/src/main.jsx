import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.jsx";
/*
src                                
 │                                 
 ├──Component(Global Level)         
 ├──Pages                          
 ├─────Page Directory              
 │        ├───Page                 
 │        ├───Component(Page Level) 
 │        └───Context & Context(Page Level)
 ├──Provider & Context(Globel Level)
 ├──assets                                                    
 │    ├───Image                 
 │    └──Icon                 
 ├──App.jsx                        
 ├──main.jsx                       
 ├──App.css                        
 ├──index.css                                                    
 └──...
 */
createRoot(document.getElementById("root")).render(
  <StrictMode>
    <App />
  </StrictMode>
);

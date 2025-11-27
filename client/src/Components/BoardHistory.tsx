import Navbar from "./Navbar";
import ThinBoard from "./ThinBoard.tsx";

export default function BoardHistory() {
    return (
        <div className="flex flex-col min-h-screen w-full">
            <Navbar/>

            {/* Example ThinBoards for demonstration, fetch from DB.
             array of selectedNumbers, creationTimestamp, expirationTimestamp. */}
            <ThinBoard selectedNumbers={[9,10,11,12,13,14,15,16 ]} creationTimestamp={1664238048} expirationTimestamp={1764238048}/>
            <ThinBoard selectedNumbers={[9,10,11,12,13]}/>
        </div>
    );
};

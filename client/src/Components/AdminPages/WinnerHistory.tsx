import Navbar from "../Navbar.tsx";
import ThinBoard from "../BoardPages/ThinBoard.tsx";
import { useWinningBoards } from "../../hooks/useWinningBoards.ts";
import type { JSX } from "react";

export default function WinnerHistory(): JSX.Element {
    const {
        winningBoards,
        loading,
        activeBoardId,
        activeBoardMatches,
        toggleBoard,
        refresh,
        page,
        totalPages,
        visibleStart,
        visibleEnd,
        total,
        handlePageChange,
    } = useWinningBoards();

    return (
        <div className="flex flex-col min-h-screen w-full bg-base-100">
            <Navbar />
            <main className="p-6 max-w-5xl mx-auto w-full space-y-6 flex-1">
                <header className="flex items-center justify-between">
                    <h1 className="text-2xl font-semibold">Vinderbræt</h1>
                    <button className="btn btn-sm" onClick={refresh} type="button">Opdater</button>
                </header>
                
                {loading ? (
                    <div className="flex items-center justify-center py-12">
                        <div className="text-center">
                            <div className="loader mb-4" />
                            <p className="text-sm text-gray-500">Henter vindende boards…</p>
                        </div>
                    </div>
                ) : (
                    <section className="space-y-4">
                        <header className="flex items-center justify-between text-sm text-gray-500">
                            <span>{total ? `Viser ${visibleStart}-${visibleEnd} af ${total}` : "Ingen vindere"}</span>
                            <div className="flex gap-2">
                                <button className="btn btn-sm" disabled={page === 1} onClick={() => handlePageChange("prev")} type="button">Tidligere</button>
                                <span className="text-sm font-medium">{`Side ${page} af ${totalPages}`}</span>
                                <button className="btn btn-sm" disabled={page >= totalPages} onClick={() => handlePageChange("next")} type="button">Næste</button>
                            </div>
                        </header>
                        {winningBoards.length === 0 ? (
                            <div className="text-center text-sm text-gray-500 py-8">Ingen vindende boards på denne side.</div>
                        ) : (
                            winningBoards.map((board) => (
                                <div key={board.winningBoardID} className="space-y-2 border rounded-lg p-4 bg-base-200">
                                    <button
                                        className="w-full text-left"
                                        onClick={() => board.winningBoardID && toggleBoard(board.winningBoardID)}
                                        type="button"
                                    >
                                        <p>Uge: {board.week}, {board.weekYear}</p>
                                        <ThinBoard
                                            selectedNumbers={(board.winningNumbers ?? []).map((entry) => Number(entry?.number ?? 0)).filter((num) => !Number.isNaN(num))}
                                            playingWeek={undefined}
                                            playingYear={undefined}
                                            hasWon={undefined}
                                            isActive={true}
                                        />
                                    </button>
                                    {activeBoardId === board.winningBoardID && (
                                        <div className="bg-base-100 rounded-lg p-3">
                                            <p className="text-sm font-medium mb-2">Matchende boards og brugere:</p>
                                            {activeBoardMatches.length === 0 ? (
                                                <p className="text-sm text-gray-500">Ingen boards fundet.</p>
                                            ) : (
                                                <ul className="space-y-3">
                                                    {activeBoardMatches.map((match) => (
                                                        <li key={match.board?.boardID ?? match.user?.userID ?? crypto.randomUUID()} className="border rounded-md p-3 text-sm">
                                                            <div className="flex flex-wrap gap-4 text-gray-700">
                                                                <span><span className="font-semibold">Board ID:</span> {match.board?.boardID ?? "Ukendt"}</span>
                                                                <span><span className="font-semibold">Bruger:</span> {match.user?.firstname} {match.user?.lastname}</span>
                                                                <span><span className="font-semibold">Email:</span> {match.user?.email ?? "Ukendt"}</span>
                                                            </div>
                                                            {Array.isArray(match.board?.numbers) && match.board?.numbers?.length ? (
                                                                <div className="mt-2 text-xs text-gray-600">
                                                                    <span className="font-semibold">Tal:</span> {match.board.numbers.map((n) => n?.number ?? "-").filter((n) => typeof n === "number").sort((a, b) => (a as number) - (b as number)).join(", ")}
                                                                </div>
                                                            ) : (
                                                                <div className="mt-2 text-xs text-gray-400">Ingen tal fundet.</div>
                                                            )}
                                                        </li>
                                                    ))}
                                                </ul>
                                            )}
                                        </div>
                                    )}
                                 </div>
                             ))
                         )}
                    </section>
                )}
            </main>
        </div>
    );
}

import React from "react";
import {useUserBoards} from "../../hooks/useUserBoards.ts";

export default function MultiSelectableBoard() {
    const {
        selected,
        toggle,
        value,
        setValue,
        getPrice,
        isValid,
        MAX_SELECTION,
        MIN_SELECTION,
        BOARD_SIZE,
        createBoard,
    } = useUserBoards();

    const options = ["1", "2", "3", "4", "5"];


    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const succeeded = await createBoard();
        if (succeeded) {
            const modal = document.getElementById("my_modal_2");
            if (modal instanceof HTMLDialogElement) {
                modal.showModal();
            }
        }
    };

    return (
        <div className="w-full max-w-lg mx-auto p-4">
            <div className="grid grid-cols-4 gap-3">
                {Array.from({ length: BOARD_SIZE }, (_, i) => i + 1).map((num) => (
                    <button
                        key={num}
                        onClick={() => toggle(num)}
                        disabled={!selected.includes(num) && selected.length >= MAX_SELECTION}
                        className={`aspect-square flex items-center justify-center rounded-xl border text-lg font-semibold transition ${
                            selected.includes(num)
                                ? "!bg-red-300 !text- !border-red-600"
                                : selected.length >= MAX_SELECTION
                                    ? "bg-gray-300 border-gray-400 cursor-not-allowed opacity-50"
                                    : "bg-base-200 border-base-300 hover:bg-base-300"
                        }`}
                    >
                        {num}
                    </button>
                ))}
            </div>
            <div className="mt-4 text-center space-y-2">
                <p className="text-lg font-semibold">Valgte numre: {[...selected].sort((a, b) => a - b).join(", ") || "Ingen"}</p>
                <p className={`text-lg font-semibold ${!isValid ? "text-red-500" : ""}`}>
                    Pris: {getPrice()} DKK {!isValid && `(${selected.length}/${MIN_SELECTION}-${MAX_SELECTION})`}
                </p>
                <div className="flex-col">
                    <div className="join">
                        {options.map((opt) => {
                            const active = value === opt;

                            return (
                                <button disabled={!isValid}
                                    key={opt}
                                    onClick={() => setValue(opt)}
                                    className={`
                                        join-item btn btn-circle btn-xl
                                        transition-all
                                        !border-red-700
                                        mx-7
                                        !rounded-full
                                        h-16 w-16
                                        !text-xl
                                        ${active
                                            ? "!bg-red-600 !text-white hover:!bg-red-700"
                                            : "!bg-red-200 !text-red-900 hover:!bg-red-300"}
                                         `}
                                >
                                    {opt}
                                </button>


                            );
                        })}
                    </div>
                </div>
                <form onSubmit={handleSubmit}>
                    <button type="submit" className="btn m-15" disabled={!isValid}>
                        Opret board
                    </button>
                </form>


            </div>
            {/* Open the modal using document.getElementById('ID').showModal() method */}
            <dialog id="my_modal_2" className="modal">
                <div className="modal-box">
                    <h3 className="font-bold text-lg">Hello!</h3>
                    <p className="py-4">Press ESC key or click outside to close</p>
                </div>
                <form method="dialog" className="modal-backdrop">
                    <button>close</button>
                </form>
            </dialog>
        </div>
    );
}
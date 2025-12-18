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
        createBoardTransaction,
    } = useUserBoards();

    const options = ["1", "2", "3", "4", "5"];
    const [customWeeks, setCustomWeeks] = React.useState("");
    const [presetFocused, setPresetFocused] = React.useState(false);

    const handlePresetSelect = (opt: string) => {
        setPresetFocused(true);
        setCustomWeeks("");
        setValue(opt);
    };

    const handleCustomWeeksChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const nextValue = event.target.value;
        if (nextValue === "") {
            setCustomWeeks("");
            setValue("");
            return;
        }

        const parsed = Number.parseInt(nextValue, 10);
        if (Number.isNaN(parsed)) {
            return;
        }

        const sanitized = Math.max(1, parsed);
        const sanitizedString = sanitized.toString();
        setCustomWeeks(sanitizedString);
        setValue(sanitizedString);
    };

    React.useEffect(() => {
        if (!value) {
            setCustomWeeks("");
        }
    }, [value]);


    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const succeeded = await createBoardTransaction();
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
                    Pris: {isValid && getPrice()} DKK {!isValid && `(${selected.length}/${MIN_SELECTION}-${MAX_SELECTION})`}
                </p>
                <div className="flex-col">
                    <p className="mb-2 text-lg font-semibold">Vælg antal trækninger(uger) (valgt: {value})</p>
                    <div className="join justify-center">

                        {options.map((opt) => {
                            const active = value === opt;

                            return (

                                <button
                                    key={opt}
                                    onClick={() => handlePresetSelect(opt)}
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
                    <div className="mt-4 flex flex-col items-center gap-2">
                        <span className="text-sm font-semibold text-red-800">Eget antal uger</span>
                        <input
                            type="number"
                            min={1}
                            aria-label="Vælg custom antal uger"
                            disabled={!isValid}
                            value={customWeeks}
                            onChange={handleCustomWeeksChange}
                            onFocus={() => setPresetFocused(false)}
                            className={`input input-bordered input-sm text-center font-semibold
                                ${(!presetFocused && customWeeks !== "") ? "!border-red-700" : ""}`}
                            placeholder="Fx 6"
                        />
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
                    <h3 className="font-bold text-lg">Plade oprettet!</h3>
                    <p className="py-4">Klik udenfor boksen for at fortsætte</p>
                </div>
                <form method="dialog" className="modal-backdrop">
                    <button>close</button>
                </form>
            </dialog>
        </div>
    );
}
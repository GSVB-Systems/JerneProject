import { useAdminBoard } from "../../hooks/useAdminBoard.ts";
import type {JSX} from "react";

export default function AdminBoard(): JSX.Element {
    const {
        BOARD_SIZE,
        MAX_SELECTION,
        MIN_SELECTION,
        selected,
        toggle,
        isValid,
        isSubmitting,
        error,
        success,
        createWinningBoard,
    } = useAdminBoard();

    const handleSubmit = async () => {
        await createWinningBoard();
    };

    return (
        <div className="w-full max-w-lg mx-auto p-4 space-y-4">
            <p className="text-xl font-semibold">Vælg denne uges vindernumre:</p>
            <p className="text-sm text-gray-500">Vælg mellem {MIN_SELECTION} og {MAX_SELECTION} af {BOARD_SIZE} numre.</p>
            <div className="grid grid-cols-4 gap-3">
                {Array.from({ length: BOARD_SIZE }, (_, i) => i + 1).map((num) => (
                    <button
                        key={num}
                        onClick={() => toggle(num)}
                        disabled={(!selected.includes(num) && selected.length >= MAX_SELECTION) || isSubmitting}
                        className={`aspect-square flex items-center justify-center rounded-xl border text-lg font-semibold transition ${
                            selected.includes(num)
                                ? "!bg-red-300 !text-black !border-red-600"
                                : selected.length >= MAX_SELECTION
                                    ? "bg-gray-300 border-gray-400 cursor-not-allowed opacity-50"
                                    : "bg-base-200 border-base-300 hover:bg-base-300"
                        }`}
                    >
                        {num}
                    </button>
                ))}
            </div>

            {error && (
                <div className="text-sm text-red-600">{error}</div>
            )}
            {success && (
                <div className="text-sm text-green-600">Vindernumrene blev gemt.</div>
            )}

            <button
                onClick={handleSubmit}
                disabled={!isValid || isSubmitting}
                className="btn w-full"
            >
                {isSubmitting ? "Gemmer..." : "Gem vindernumre"}
            </button>
        </div>
    );
}
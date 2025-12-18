export default function ThinBoard({
    selectedNumbers,
    playingWeek,
    playingYear,
    hasWon,
    isActive,
}: {
    selectedNumbers: number[];
    playingWeek?: number;
    playingYear?: number;
    hasWon?: boolean;
    isActive?: boolean;
}) {
    const hasPlayingPeriod = typeof playingWeek === "number" && typeof playingYear === "number";
    const numberClasses = isActive
        ? "border-red-600 bg-red-300 text-gray-900"
        : "border-gray-400 bg-gray-200 text-gray-500";

    const getWeekAndYear = () => {
        if (!hasPlayingPeriod) return "";
        return `Uge ${playingWeek}, ${playingYear}`;
    };

    const boardWrapperClasses = `border-2 rounded-lg h-16 sm:h-20 flex items-center justify-center overflow-x-auto flex-1 ${
        hasWon ? "border-green-500 bg-green-50 shadow-lg" : "border-gray-400 bg-gray-50"
    }`;

    return (
        <div className="w-full flex items-center justify-center p-4">
            <div className="w-full max-w-2xl">
                {hasPlayingPeriod && (
                    <div className="text-center text-sm font-semibold mb-2 text-gray-700">
                        {getWeekAndYear()}
                    </div>
                )}

                <div className={`${boardWrapperClasses} relative`}>
                    {hasWon && (
                        <div className="absolute -mt-12 self-start text-xs font-semibold text-green-700" aria-live="polite">
                            <span className="px-2 py-1 bg-green-100 border border-green-500 rounded-full">Winner</span>
                            <span className="sr-only">This board has already won</span>
                        </div>
                    )}

                    <div className="flex gap-2">
                        <div className="flex items-center justify-center gap-4">
                            {[...selectedNumbers].sort((a, b) => a - b).map((num) => (
                                <div
                                    key={num}
                                    className={`w-8 h-8 sm:w-12 sm:h-12 flex-shrink-0 flex items-center justify-center rounded-lg border-2 text-sm sm:text-lg font-bold ${numberClasses}`}
                                >
                                    {num}
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
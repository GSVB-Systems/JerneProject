export default function ThinBoard({
    selectedNumbers,
    creationTimestamp,
    expirationTimestamp
}: {
    selectedNumbers: number[];
    creationTimestamp?: number;
    expirationTimestamp?: number;
}) {
    const getWeekAndYear = (unix?: number) => {
        if (!unix) return "";
        const date = new Date(unix * 1000);
        const yearStart = new Date(date.getFullYear(), 0, 1);
        const weekNumber = Math.ceil((date.getTime() - yearStart.getTime()) / 86400000 / 7);
        const year = date.getFullYear();
        return `Uge ${weekNumber}, ${year}`;
    };

    const getWeeksRemaining = (unix?: number) => {
        if (!unix) return 0;
        const now = new Date();
        const target = new Date(unix * 1000);
        const weeksRemaining = Math.ceil((target.getTime() - now.getTime()) / (7 * 24 * 60 * 60 * 1000));
        return Math.max(0, weeksRemaining);
    };

    return (
        <div className="w-full flex items-center justify-center p-4">
            <div className="w-full max-w-2xl">
                {creationTimestamp && (
                    <div className="text-center text-sm font-semibold mb-2 text-gray-700">
                        {getWeekAndYear(creationTimestamp)}
                    </div>
                )}

                    <div className="border-2 border-gray-400 rounded-lg bg-gray-50 h-16 sm:h-20 flex items-center justify-center overflow-x-auto flex-1">
                        <div className="flex gap-2">
                            <div className="flex items-center justify-center gap-4">
                                {expirationTimestamp && (
                                    <div className="text-3xl sm:text-5xl font-bold text-gray-800 text-center">
                                        {getWeeksRemaining(expirationTimestamp)}
                                    </div>
                                )}
                            {[...selectedNumbers].sort((a, b) => a - b).map((num) => (
                                <div
                                    key={num}
                                    className="w-8 h-8 sm:w-12 sm:h-12 flex-shrink-0 flex items-center justify-center rounded-lg border-2 border-red-600 bg-red-300 text-sm sm:text-lg font-bold"
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
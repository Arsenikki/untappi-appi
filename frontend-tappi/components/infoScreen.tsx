import React from 'react';

interface Props {
  showInfo: boolean;
  handleShowInfo: () => void;
  darkMode: boolean;
  handleToggleDarkMode: () => void;
}

const InfoScreen: React.FC<Props> = ({ showInfo, handleShowInfo, darkMode, handleToggleDarkMode }): JSX.Element => (
  <>
    {showInfo ? (
      <>
        <div className="justify-center items-center flex overflow-x-hidden overflow-y-auto fixed inset-0 z-50 outline-none focus:outline-none">
          <div className="relative w-auto pointer-events-auto m-8 max-w-2xl">
            {/* content */}
            <div className="border-0 rounded-lg shadow-lg relative flex flex-col w-full bg-white outline-none focus:outline-none">
              {/* header */}
              <div className="flex items-start justify-between p-5 border-b border-solid border-gray-300 rounded-t">
                <h3 className="font-light font-mono text-3xl">Untappi-appi</h3>
              </div>
              {/* body */}
              <div className="relative p-6 flex-auto">
                <p className="my-4 text-gray-600 text-lg leading-relaxed">Check top beers from nearby venues</p>
                <p className="my-4 text-gray-600 text-lg leading-relaxed">
                  Live location can be enabled from top right corner
                </p>
              </div>
              {/* footer */}
              <div className="flex items-center justify-between p-6 border-t border-solid border-gray-300 rounded-b">
                <div className="flex flex-col justify-between w-2/3">
                  <label className="flex items-center">
                    <input
                      type="checkbox"
                      className="form-checkbox"
                      onChange={() => handleToggleDarkMode()}
                      checked={darkMode}
                    />
                    <span className="ml-2">Dark mode</span>
                  </label>
                  <label className="flex items-center">
                    <input type="checkbox" className="form-checkbox" />
                    <span className="ml-2">Don't show again</span>
                  </label>
                </div>
                <button
                  className="bg-gray-800 text-white active:bg-green-600 font-bold uppercase text-sm px-6 py-3 rounded shadow hover:shadow-lg hover:bg-gray-700 outline-none focus:outline-none mr-1 mb-1"
                  type="button"
                  style={{ transition: 'all .15s ease' }}
                  onClick={() => handleShowInfo()}
                >
                  Oispa kaljaa?
                </button>
              </div>
            </div>
          </div>
        </div>
        <div className="opacity-25 fixed inset-0 z-40 bg-black" />
      </>
    ) : null}
  </>
);

export default InfoScreen;

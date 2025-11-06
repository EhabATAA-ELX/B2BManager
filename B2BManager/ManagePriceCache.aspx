<%@ Page Title="Manage Price Cache" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="ManagePriceCache.aspx.vb" Inherits="ManagePriceCache" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <script src="https://cdn.tailwindcss.com"></script>
    <style>
        /* Styles moved from body to here, as body is in the master page */
        #pageWrapper { 
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif; 
            background-color: #F9FAFB; /* bg-gray-50 */
        }
        .form-input {
            appearance: none;
            border-radius: 0.375rem;
            border-width: 1px;
            border-color: #D1D5DB; /* gray-300 */
            padding: 0.5rem 0.75rem;
            font-size: 1rem;
            line-height: 1.5rem;
            width: 100%;
        }
        .form-input:focus {
            outline: 2px solid transparent;
            outline-offset: 2px;
            border-color: #2563EB; /* blue-600 */
            box-shadow: 0 0 0 2px #BFDBFE; /* blue-200 */
        }
        .form-checkbox {
            border-radius: 0.25rem;
            border-color: #D1D5DB; /* gray-300 */
            color: #2563EB; /* blue-600 */
        }
        .form-checkbox:focus {
             box-shadow: 0 0 0 2px #BFDBFE; /* blue-200 */
             border-color: #2563EB; /* blue-600 */
        }
        .btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            padding: 0.5rem 1rem;
            font-size: 1rem;
            font-weight: 500;
            border-radius: 0.375rem;
            border-width: 1px;
            border-color: transparent;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
            transition: all 0.2s ease-in-out;
            cursor: pointer;
        }
        .btn:disabled {
            opacity: 0.5;
            cursor: not-allowed;
        }
        .btn-primary {
            background-color: #2563EB; /* blue-600 */
            color: white;
        }
        .btn-primary:hover:not(:disabled) {
            background-color: #1D4ED8; /* blue-700 */
        }
        
        /* Loading Spinner */
        #loader {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(255, 255, 255, 0.7);
            z-index: 9999;
        }
        #loader div {
            position: absolute;
            top: 50%;
            left: 50%;
            width: 4rem;
            height: 4rem;
            margin-top: -2rem;
            margin-left: -2rem;
            border: 4px solid #D1D5DB; /* gray-300 */
            border-top-color: #2563EB; /* blue-600 */
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }
        @keyframes spin {
            to { transform: rotate(360deg); }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:ScriptManager runat="server" ID="ScriptManager1" EnablePageMethods="true" />
    <div id="pageWrapper" class="p-6 max-w-4xl mx-auto">
       

        <div id="loader"><div></div></div>

        <h1 class="text-2xl font-semibold text-gray-800 mb-6">Price Cache Management</h1>

        <!-- Common Parameters -->
        <fieldset class="bg-white p-5 rounded-lg shadow-md mb-6 border border-gray-200">
            <legend class="text-lg font-medium text-gray-700 px-2 -ml-2">Common Parameters</legend>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-4">
                <div>
                    <label for="txtApiKey" class="block text-sm font-medium text-gray-600 mb-1">X-API-Key</label>
                    <asp:TextBox ID="txtApiKey" runat="server" CssClass="form-input" TextMode="Password" />
                    <p class="text-xs text-gray-500 mt-1">Your API key. Will not be stored.</p>
                </div>
                <div>
                    <label for="txtSopIndicator" class="block text-sm font-medium text-gray-600 mb-1">SOP Indicator (e.g., DGM)</label>
                    <asp:TextBox ID="txtSopIndicator" runat="server" CssClass="form-input" Text="DGM" />
                </div>
            </div>
        </fieldset>

        <!-- API Actions -->
        <div class="space-y-6">

            <!-- 1. GetPrice BypassCacheOrNot -->
            <fieldset class="bg-white p-5 rounded-lg shadow-md border border-gray-200">
                <legend class="text-lg font-medium text-blue-700 px-2 -ml-2">1. Get Price (Bypass Cache)</legend>
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mt-4">
                    <div>
                        <label for="txtGetPrice_CustomerNumber" class="block text-sm font-medium text-gray-600 mb-1">Customer Number</label>
                        <asp:TextBox ID="txtGetPrice_CustomerNumber" runat="server" CssClass="form-input bg-gray-100" ReadOnly="true" />
                    </div>
                     <div>
                        <label for="txtGetPrice_EmailLogin" class="block text-sm font-medium text-gray-600 mb-1">Email Login</label>
                        <asp:TextBox ID="txtGetPrice_EmailLogin" runat="server" CssClass="form-input" Text="hamsharubini.u@cognizant.com" />
                    </div>
                    <div>
                        <label for="txtGetPrice_ProductCode" class="block text-sm font-medium text-gray-600 mb-1">Product Code</label>
                        <asp:TextBox ID="txtGetPrice_ProductCode" runat="server" CssClass="form-input" Text="940321379" />
                    </div>
                     <div>
                        <label for="txtGetPrice_Quantity" class="block text-sm font-medium text-gray-600 mb-1">Quantity</label>
                        <asp:TextBox ID="txtGetPrice_Quantity" runat="server" CssClass="form-input" Text="1" type="number" />
                    </div>
                    <div>
                        <label for="txtGetPrice_Language" class="block text-sm font-medium text-gray-600 mb-1">Language</label>
                        <asp:TextBox ID="txtGetPrice_Language" runat="server" CssClass="form-input" Text="DE" />
                    </div>
                    <div>
                        <label for="txtGetPrice_Currency" class="block text-sm font-medium text-gray-600 mb-1">Currency</label>
                        <asp:TextBox ID="txtGetPrice_Currency" runat="server" CssClass="form-input" Text="EUR" />
                    </div>
                </div>
                <div class="flex items-center space-x-6 mt-6">
                    <div class="flex items-center">
                        <asp:CheckBox ID="chkGetPrice_BypassCache" runat="server" CssClass="form-checkbox h-5 w-5" Text=" " Checked="true" />
                        <label for="<%= chkGetPrice_BypassCache.ClientID %>" class="ml-2 block text-sm font-medium text-gray-700">Bypass Cache</label>
                    </div>
                    <div class="flex items-center">
                        <asp:CheckBox ID="chkGetPrice_UpdateCache" runat="server" CssClass="form-checkbox h-5 w-5" Text=" " Checked="true" />
                        <label for="<%= chkGetPrice_UpdateCache.ClientID %>" class="ml-2 block text-sm font-medium text-gray-700">Update Cache</label>
                    </div>
                </div>
                <div class="text-right mt-4">
                    <asp:Button ID="btnGetPrice" runat="server" Text="Call API" OnClientClick="callGetPrice(); return false;" CssClass="btn btn-primary" />
                </div>
            </fieldset>

            <!-- 2. Renew Cache (Specific Customer) -->
            <fieldset class="bg-white p-5 rounded-lg shadow-md border border-gray-200">
                <legend class="text-lg font-medium text-blue-700 px-2 -ml-2">2. Renew Cache (Specific Customer)</legend>
                <p class="text-sm text-gray-600 mt-4">This action will use the <strong>SOP Indicator</strong> from the common parameters and the <strong>Customer Number</strong> below.</p>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-4">
                     <div>
                        <label for="txtRenew_CustomerNumber" class="block text-sm font-medium text-gray-600 mb-1">Customer Number</label>
                        <asp:TextBox ID="txtRenew_CustomerNumber" runat="server" CssClass="form-input bg-gray-100" ReadOnly="true" />
                    </div>
                </div>
                <div class="text-right mt-4">
                    <asp:Button ID="btnRenewCache" runat="server" Text="Call API" OnClientClick="callRenewCache(); return false;" CssClass="btn btn-primary" />
                </div>
            </fieldset>

            <!-- 3. Renew Cache (Customer Range) -->
            <fieldset class="bg-white p-5 rounded-lg shadow-md border border-gray-200">
                <legend class="text-lg font-medium text-blue-700 px-2 -ml-2">3. Renew Price Cache (Customer Range)</legend>
                 <p class="text-sm text-gray-600 mt-4">This action will use the <strong>SOP Indicator</strong> from the common parameters and the <strong>Customer Number</strong> below.</p>
                 <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-4">
                     <div>
                        <label for="txtRenewRange_CustomerNumber" class="block text-sm font-medium text-gray-600 mb-1">Customer Number</label>
                        <asp:TextBox ID="txtRenewRange_CustomerNumber" runat="server" CssClass="form-input bg-gray-100" ReadOnly="true" />
                    </div>
                </div>
                <div class="text-right mt-4">
                    <asp:Button ID="btnRenewRange" runat="server" Text="Call API" OnClientClick="callRenewRange(); return false;" CssClass="btn btn-primary" />
                </div>
            </fieldset>

            <!-- 4. Delete Cache -->
            <fieldset class="bg-white p-5 rounded-lg shadow-md border border-gray-200">
                <legend class="text-lg font-medium text-red-700 px-2 -ml-2">4. Delete Cache (Specific Customer)</legend>
                 <p class="text-sm text-gray-600 mt-4">This action will use the <strong>SOP Indicator</strong> from the common parameters and the <strong>Customer Number</strong> below.</p>
                 <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-4">
                     <div>
                        <label for="txtDelete_CustomerNumber" class="block text-sm font-medium text-gray-600 mb-1">Customer Number</label>
                        <asp:TextBox ID="txtDelete_CustomerNumber" runat="server" CssClass="form-input bg-gray-100" ReadOnly="true" />
                    </div>
                </div>
                <div class="text-right mt-4">
                    <asp:Button ID="btnDeleteCache" runat="server" Text="Call API" OnClientClick="callDeleteCache(); return false;" CssClass="btn btn-primary bg-red-600 hover:bg-red-700" />
                </div>
            </fieldset>

        </div>

        <!-- Result Panel -->
        <div class="mt-8">
            <h3 class="text-lg font-medium text-gray-800 mb-2">API Response</h3>
            <div id="resultPanel" class="bg-gray-900 text-white p-4 rounded-lg font-mono text-sm overflow-x-auto min-h-[150px]">
                <pre id="resultPre">Waiting for API call...</pre>
            </div>
        </div>

    </div> <%-- End wrapper div --%>

    <script type="text/javascript">
        // Helper to get element by ID cleanly
        function $(id) {
            return document.getElementById(id);
        }

        // --- Global Helper Functions ---
        function showLoading(isLoading) {
            var loader = $('loader');
            var buttons = document.querySelectorAll('.btn');
            if (isLoading) {
                loader.style.display = 'block';
                buttons.forEach(function (btn) { btn.disabled = true; });
            } else {
                loader.style.display = 'none';
                buttons.forEach(function (btn) { btn.disabled = false; });
            }
        }

        function showResult(resultText, isError) {
            var pre = $('resultPre');
            var panel = $('resultPanel');
            var result = {};

            try {
                // Try to parse the result as JSON for pretty printing
                result = JSON.parse(resultText);
                pre.textContent = JSON.stringify(result, null, 2); // Pretty print
            } catch (e) {
                // If not JSON, just show the raw text
                pre.textContent = resultText;
                result = { error: resultText }; // Create a dummy error object
            }

            // Style the panel based on success or error
            if (isError || result.error || (result.StatusCode && result.StatusCode >= 400)) {
                panel.classList.remove('bg-gray-900');
                panel.classList.add('bg-red-900');
                panel.classList.remove('text-white');
                panel.classList.add('text-red-100');
            } else {
                panel.classList.remove('bg-red-900');
                panel.classList.add('bg-gray-900');
                panel.classList.remove('text-red-100');
                panel.classList.add('text-white');
            }
        }

        function getCommonParams() {
            var apiKey = $_asp('<%= txtApiKey.ClientID %>').value;
            var sopIndicator = $_asp('<%= txtSopIndicator.ClientID %>').value;

            if (!apiKey || !sopIndicator) {
                showResult(JSON.stringify({ error: "X-API-Key and SOP Indicator are required." }), true);
                return null;
            }
            return { apiKey: apiKey, sopIndicator: sopIndicator };
        }

        // Helper to get ASP.NET control by ID
        function $_asp(id) {
            return document.getElementById(id);
        }

        // Helper: start job and poll until completion
        function startJobAndPoll(apiKey, endpoint, jsonBody) {
            showLoading(true);
            PageMethods.StartLongRunningCall(apiKey, endpoint, jsonBody,
                function (jobId) {
                    pollJob(jobId, Date.now());
                },
                function (err) {
                    showLoading(false);
                    showResult(err.get_message(), true);
                }
            );
        }

        var POLL_INTERVAL_MS = 3000; // 3s
        var JOB_TIMEOUT_MS = 15 * 60 * 1000; // 15 minutes

        function pollJob(jobId, startTime) {
            var startTs = startTime || Date.now();
            if (Date.now() - startTs > JOB_TIMEOUT_MS) {
                showLoading(false);
                showResult(JSON.stringify({ error: 'Job timed out' }), true);
                return;
            }
            PageMethods.GetJobStatus(jobId,
                function (statusJson) {
                    try {
                        var s = typeof statusJson === 'string' ? JSON.parse(statusJson) : statusJson;
                        if (s.status === 'Pending' || s.status === 'Running') {
                            setTimeout(function () { pollJob(jobId, startTs); }, POLL_INTERVAL_MS);
                        } else if (s.status === 'Completed') {
                            showLoading(false);
                            showResult(s.result, false);
                        } else if (s.status === 'Failed') {
                            showLoading(false);
                            showResult(s.error || 'Job failed', true);
                        } else {
                            showLoading(false);
                            showResult(JSON.stringify({ error: 'Unknown job status' }), true);
                        }
                    } catch (ex) {
                        showLoading(false);
                        showResult(ex.toString(), true);
                    }
                },
                function (err) {
                    showLoading(false);
                    showResult(err.get_message(), true);
                }
            );
        }

        // --- API Call Handlers: use long-running job flow ---
        function callGetPrice() {
            var common = getCommonParams(); if (!common) return;
            var payload = {
                EmailLogin: $_asp('<%= txtGetPrice_EmailLogin.ClientID %>').value,
                SopId: common.sopIndicator,
                CustomerNumber: $_asp('<%= txtGetPrice_CustomerNumber.ClientID %>').value,
                Language: $_asp('<%= txtGetPrice_Language.ClientID %>').value,
                Currency: $_asp('<%= txtGetPrice_Currency.ClientID %>').value,
                ProductCode: $_asp('<%= txtGetPrice_ProductCode.ClientID %>').value,
                RequestedQuantity: parseInt($_asp('<%= txtGetPrice_Quantity.ClientID %>').value, 10) || 1,
                BypassCache: $_asp('<%= chkGetPrice_BypassCache.ClientID %>').checked,
                UpdateCache: $_asp('<%= chkGetPrice_UpdateCache.ClientID %>').checked
            };
            startJobAndPoll(common.apiKey, 'PriceBypassCache', JSON.stringify(payload));
        }

        function callRenewCache() {
            var common = getCommonParams(); if (!common) return;
            var payload = { SopIndicator: common.sopIndicator, customerNumber: $_asp('<%= txtRenew_CustomerNumber.ClientID %>').value };
            startJobAndPoll(common.apiKey, 'RenewCustomerCache', JSON.stringify(payload));
        }

        function callRenewRange() {
            var common = getCommonParams(); if (!common) return;
            var payload = { SopIndicator: common.sopIndicator, CustomerNumber: $_asp('<%= txtRenewRange_CustomerNumber.ClientID %>').value };
            startJobAndPoll(common.apiKey, 'PriceForCustomerRange', JSON.stringify(payload));
        }

        function callDeleteCache() {
            var common = getCommonParams(); if (!common) return;
            var payload = { SopIndicator: common.sopIndicator, CustomerNumber: $_asp('<%= txtDelete_CustomerNumber.ClientID %>').value };
            startJobAndPoll(common.apiKey, 'Cache', JSON.stringify(payload));
        }

    </script>
</asp:Content>
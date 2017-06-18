using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neth2RC4
{
    public class Contract
    {
        public string Code { get; set; }
        public string Abi { get; set; }
        public string ByteCode { get; set; }
        public string SenderAddress { get; set; } = "0x65873e9f02633f8b87ca1896bd811c58ad000a15";
        public string Password { get; set; } = "password";

        public TransactionReceipt Receipt { get; set; }

        private Web3Geth Web3;

        public Contract(string code, string senderAddress, string password)
        {
            this.Code = code;
            this.SenderAddress = senderAddress;
            this.Password = password;

            this.Web3 = new Web3Geth();
        }

        public void Compile()
        {
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = @"D:\Prasanna\BlockChain\Repos\Compiler\solc.exe",
                    Arguments = @"--bin --abi -",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            StreamWriter sw = proc.StandardInput;
            sw.WriteLine(this.Code);
            sw.Dispose();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                // do something with line
                if (line == "Binary: ")
                {
                    this.ByteCode = "0x" + proc.StandardOutput.ReadLine();
                }
                else if (line == "Contract JSON ABI")
                {
                    this.Abi = proc.StandardOutput.ReadLine();
                }
            }
            proc.Dispose();
        }

        public async Task Deploy()
        {
            var unlockAccountResult =
                await this.Web3.Personal.UnlockAccount.SendRequestAsync(this.SenderAddress, this.Password, 120);

            var transactionHash =
                await this.Web3.Eth.DeployContract.SendRequestAsync(this.Abi, this.ByteCode, this.SenderAddress, new HexBigInteger(300000));

            this.Receipt = await this.Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            while (this.Receipt == null)
            {
                Thread.Sleep(5000);
                this.Receipt = await this.Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }
        }

        public async Task<T> Invoke<T>(string functionName, params object[] functionInput)
        {
            var contract = this.Web3.Eth.GetContract(this.Abi, this.Receipt.ContractAddress);

            var multiplyFunction = contract.GetFunction(functionName);

            return await multiplyFunction.CallAsync<T>(functionInput);
        }
    }
}

using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neth2RC4
{
    public class MultiplyTest
    {
        public async Task Run()
        {
            var senderAddress = "0x0bd615694d433424c292599f080bda45b0c257fc";
            var password = "password";
            var abi = @"[{""constant"":true,""inputs"":[{""name"":""num"",""type"":""uint256""}],""name"":""multiply"",""outputs"":[{""name"":""ret"",""type"":""uint256""}],""payable"":false,""type"":""function""},{""constant"":true,""inputs"":[],""name"":""greet"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""type"":""function""}]";
            var byteCode =
                "0x6060604052341561000c57fe5b5b6101838061001c6000396000f300606060405263ffffffff7c0100000000000000000000000000000000000000000000000000000000600035041663c6888fa18114610045578063cfae32171461006a575bfe5b341561004d57fe5b6100586004356100fa565b60408051918252519081900360200190f35b341561007257fe5b61007a610104565b6040805160208082528351818301528351919283929083019185019080838382156100c0575b8051825260208311156100c057601f1990920191602091820191016100a0565b505050905090810190601f1680156100ec5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b600781025b919050565b61010c610145565b5060408051808201909152600881527f50726173616e6e6100000000000000000000000000000000000000000000000060208201525b90565b604080516020810190915260008152905600a165627a7a723058202b20532a89bde177da0d839409a94dab6c1e33af3295afe0c8e9610d1de3e8f30029";

            var web3 = new Web3Geth();
            var unlockAccountResult =
                await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, 120);
            Console.WriteLine("Unlock Account: " + unlockAccountResult);

            var transactionHash =
                await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, senderAddress, new HexBigInteger(300000));
            Console.WriteLine("Deploy Contract: " + transactionHash);

            // if geth is started with mining then no need to start the mining
            await web3.Miner.Start.SendRequestAsync(6);
            Console.WriteLine("Miner Started");

            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            while (receipt == null)
            {
                Thread.Sleep(5000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }
            Console.WriteLine("Contract Deployment: " + receipt.ContractAddress);

            var mineResult = await web3.Miner.Stop.SendRequestAsync();
            Console.WriteLine("Miner Stopped : " + mineResult);

            var contractAddress = receipt.ContractAddress;
            var contract = web3.Eth.GetContract(abi, contractAddress);

            var multiplyFunction = contract.GetFunction("multiply");

            var result = await multiplyFunction.CallAsync<int>(40);

            Console.WriteLine("Result : " + result);
        }
    }
}

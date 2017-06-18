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
            string code = @"
pragma solidity ^0.4.11;

contract Multiplier {
    function multiply(uint num) constant returns (uint ret) {
        ret = num * 7;
    }
}
            ";
            string senderAddress = "0x0bd615694d433424c292599f080bda45b0c257fc";
            string password = "password";

            Contract contract = new Contract(code, senderAddress, password);

            contract.Compile();
            Console.WriteLine("Contract Compiled");

            await contract.Deploy();
            Console.WriteLine("Contract Deployed at " + contract.Receipt.ContractAddress);

            int result = await contract.Invoke<int>("multiply", 40);
            Console.WriteLine("Invoke Result : " + result);
        }
    }
}

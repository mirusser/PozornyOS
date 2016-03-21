using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationSystem_DiscSystem.Module_4.Source.FolderSystem
{
    class FileClass
    {
        private String FileName;
        private byte FirstFileBlock;
        private byte LastFileBlock;
        private int FileSize;

        public FileClass()
        {
            FileSize = 1;
        }
        public void setFileName(String fileName)
        {
            this.FileName = fileName;
        }
        public String getFileName()
        {
            return this.FileName;
        }
        public void setFirstFileBlock(byte firstFileBlock)
        {
            this.FirstFileBlock = firstFileBlock;
        }
        public byte getFirstFileBlock()
        {
            return this.FirstFileBlock;
        }
        public void setLastFileBlock(byte lastFileBlock)
        {
            this.LastFileBlock = lastFileBlock;
        }
        public byte getLastFileBlock()
        {
            return this.LastFileBlock;
        }
        public void setFileSize(int fileSize)
        {
            this.FileSize = fileSize;
        }
        public int getFileSize()
        {
            return this.FileSize;
        }
        public void updateFileSize()
        {
            FileSize++;
        }
        public void updateFileSize(int size)
        {
            FileSize += size;
        }
    }
}
